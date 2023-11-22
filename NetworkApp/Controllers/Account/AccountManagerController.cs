using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NetworkApp.Data;
using NetworkApp.Data.Repository;
using NetworkApp.Data.UoW;
using NetworkApp.Extentions;
using NetworkApp.Models.Users;
using NetworkApp.ViewModels;
using NetworkApp.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NetworkApp.Controllers.Account
{
    public class AccountManagerController : Controller
    {
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private IUnitOfWork _unitOfWork;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {

            var usergen = new GenetateUsers();
            var userlist = usergen.Populate(10);

            foreach(var user in userlist)
            {
                var result = await _userManager.CreateAsync(user, "1");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var model = new UserViewModel(result);
            model.Friends = GetAllFriend(model.User);

            return View("User", model);
        }

        private List<User> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;
            var result = await _userManager.GetUserAsync(user);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(result);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var user = User;

            var result = _userManager.GetUserAsync(user);

            var editmodel = _mapper.Map<UserEditViewModel>(result.Result);

            return View("Edit", editmodel);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                user.Convert(model);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    return RedirectToAction("Edit", "AccountManager");
                }
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
                return View("Edit", model);
            }
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var user = _mapper.Map<User>(model);
                var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                   return RedirectToAction("MyPage", "AccountManager");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("UserList")]
        [HttpGet]
        public async Task<IActionResult> UserList(string search)
        {
            var model = await CreateSearch(search);
            return View("UserList", model);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.AddFriend(result, friend);


            return RedirectToAction("MyPage", "AccountManager");
        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;
            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;
            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var list = _userManager.Users.AsEnumerable().ToList();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.GetFullName().ToLower().Contains(search.ToLower())).ToList();
            }
            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            list.ForEach(x =>
            {
                var t = _mapper.Map<UserWithFriendExt>(x);
                t.IsFriendWithCurrent = withfriend.Where(y => y.Id == x.Id || x.Id == result.Id).Count() != 0;
                data.Add(t);
            });

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }


        [Route("ChatMessageList")]
        [HttpGet]
        public async Task<IActionResult> ChatMessageList(string id)
        {
            var model = await GenerateChat(id);
            //string st = "Chat?id=401b8c06-2730-406c-8edd-441aa94f121d";
            //string st1 = "@\"<iframe name='myIframe' id='myIframe' width='100%' height='300' src='ChatMessageList?id=" + id + "'></iframe>\"";
            //ViewData["Iframe"] = @"<iframe name='myIframe' id='myIframe' width='100%' height='300' src='@st'></iframe>";
            //ViewData["Iframe"] = st1;

            return View("ChatMessageList", model);
        }



        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var model = await GenerateChat(id);
            //string st1 = "@\"<iframe name='myIframe' id='myIframe' width='100%' height='300' src='ChatMessageList?id=" + id + "'></iframe>\"";
            //ViewData["Iframe"] = @"<iframe name='myIframe' id='myIframe' width='100%' height='300' src='@st'></iframe>";
            //ViewData["Iframe"] = st1;

            return View("Chat", model);
        }

        private async Task<ChatViewModel> GenerateChat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;
            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };

            return model;
        }

        //[Route("Chat")]
        //[HttpGet]
        //public async Task<IActionResult> Chat()
        //{
        //    var id = Request.Query["id"];
        //    var model = await GenerateChat(id);
        //    return View("Chat", model);
        //}

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            if (!String.IsNullOrEmpty(chat.NewMessage.Text))
            {
                var currentuser = User;

                var result = await _userManager.GetUserAsync(currentuser);
                var friend = await _userManager.FindByIdAsync(id);

                var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

                var item = new Message()
                {
                    Sender = result,
                    Recipient = friend,
                    Text = chat.NewMessage.Text,
                };
                repository.Create(item);
                //repository.Create(item);
                ModelState.Clear(); 
            }

            var model = await GenerateChat(id);
            return View("Chat", model);
        }
    }
}
