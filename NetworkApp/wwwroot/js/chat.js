"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (fromUser, toUser, message) {

    var currentUser = document.getElementById("ModelYouId").innerHTML;
    var chatWith = document.getElementById("ModelToWhomId").innerHTML;

    if (fromUser == chatWith && toUser == currentUser) {
        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);
        li.textContent = `${message}`;
    }
});

connection.start().then(function () {
    document.getElementById("sendButton1").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton1").addEventListener("click", function (event) {
    var fromUser = document.getElementById("ModelYouId").innerHTML;
    var toUser = document.getElementById("ModelToWhomId").innerHTML;
    var message = document.getElementById("msgText").value;
    if (message != "") {
        connection.invoke("SendMessage", fromUser, toUser, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
});