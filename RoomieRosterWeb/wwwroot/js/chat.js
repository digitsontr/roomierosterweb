const isOnChatPage = window.location.pathname.toLowerCase().includes('/chat');

const checkConnectionAndRun = (callback, count) => {
    setTimeout(() => {
        if ((window.connection || {}).state !== 'Connected' && count !== 0) {
            checkConnectionAndRun(callback, count - 1)
        } else if (typeof callback === 'function') {
            callback();
        }
    }, 500);
}

const connectionBuild = () => {
    window.connection = new signalR.HubConnectionBuilder().withUrl(window.config.baseUrl + "chat", {
        accessTokenFactory: () => window.config.accessToken
    }).build();

    window.connection.start().then(function () {
        if (isOnChatPage) {
            onPreviousMessages();
            getChats();
            onUserChats();
        } else {
            $('.spinner').css('visibility', 'hidden');
        }

        onNewMessage();
        
    }).catch(function (err) {
        return console.error(err.toString());
        $('.spinner').css('visibility', 'hidden');
    });
}


const bindChatEvents = () => {
    $('.messages-box .list-group-item').off('click').on('click',
        function (event) {
            const chatId = $(this).attr('data-chat-id');
            $('#selectedChatId').val(chatId);

            $('.messages-box .list-group-item.active').removeClass('active');
            $(this).addClass('active');

            getMessagesByChat(chatId);

            bindChatEvents();
        });

    $('#sendMessage').off('click').on('click', function () {
        const recieverName = $('.messages-box > .list-group > .list-group-item[data-chat-id=' + $('#selectedChatId').val() + ']').attr('data-username') || '';
        const message = $('.messageInput').val();

        $('.messageInput').val('');

        sendPrivate(recieverName, message);
        bindChatEvents();
    });

    $('.messageInput').off('keypress').on('keypress', function (e) {
        if (e.keyCode === 13) {
            const recieverName = $('.messages-box > .list-group > .list-group-item[data-chat-id=' + $('#selectedChatId').val() + ']').attr('data-username') || '';
            const message = $('.messageInput').val();
            $('.messageInput').val('');

            sendPrivate(recieverName, message);
            bindChatEvents();
        }
    });

    $('form').off('submit').on('submit', function (e) {
        e.preventDefault();
    });
};

const renderChatBox = (messageList, chatInfo) => {
    $('.chat-box > div').remove();

    messageList.map((message) => {
        const messageTime = new Date(message.createdAt).toLocaleDateString('tr-TR', {
            hour: 'numeric',
            minute: 'numeric'
        }).split(' ').slice(-1)[0];
        const messageDate = new Date(message.createdAt).toLocaleDateString('tr-TR', {
            month: 'long',
            day: 'numeric'
        });
        const isReciever = message.senderUserName !== $('#username').val();

        const messageBox = `
            <div class="media w-50 ${!isReciever ? 'ms-auto': ''} mb-3" bis_skin_checked="1">
                <div class="media-body" bis_skin_checked="1">
                    <div class="${!isReciever ? 'bg-custom-1' : 'border border-dark'} rounded py-2 px-3 mb-2" bis_skin_checked="1">
                        <p class="text-small mb-0 ${!isReciever ? 'text-white' : ''}">${message.content}</p>
                    </div>
                    <p class="small text-muted">${messageTime} | ${messageDate}</p>
                </div>
             </div>`;

        $(messageBox).insertBefore('.chat-box > form');
    });

    $('.chat-box').prev().find('.username').text(chatInfo.recieverFullName);
    $('.chat-box').prev().find('img').attr('src', chatInfo.reciverProfilePhoto);
    $('.chat-box')[0].scrollTop = $('.chat-box')[0].scrollHeight; //TODO TEST HERE
    $('.spinner').css('visibility', 'hidden');
};

const renderMessageBoxes = (userBoxes) => {
    $('.messages-box > .list-group >').remove();

    userBoxes.map((userBox, key) => {
        let date = new Date(userBox.lastMessageDate).toLocaleDateString('tr-TR', {
            day: 'numeric',
            month: 'long'
        });

        if (date.indexOf('Invalid') !== -1) {
            date = new Date().toLocaleDateString('tr-TR', {
                day: 'numeric',
                month: 'long'
            });
        }

        const userBoxHTML = `
                    <a class="list-group-item list-group-item-action text-white rounded - 0" data-chat-id="${userBox.id}" data-username=${userBox.recieverUserName}>
                        <div class="media">
                            <img src="data:image/jpeg;base64, ${userBox.recieverProfilePhoto}" alt="user" width="50" class="rounded-circle user-profile-image">
                            <div class="media-body ml-4">
                                <div class="d-flex align-items-center justify-content-between mb-1">
                                    <h6 class="mb-0 username">${userBox.recieverFullName}</h6><small class="small font-weight-bold">${date}</small>
                                </div>
                                <p class="font-italic mb-0 text-small">${userBox.lastMessage || '&nbsp;<br />&nbsp;'}</p>
                            </div>
                        </div>
                    </a>`;
        if (key === 0) {
            $('#selectedChatId').val(userBox.id);
        }

        $('.messages-box > .list-group').append(userBoxHTML);
        $('.messages-box > .list-group a').removeClass('text-white');
        $('.messages-box > .list-group a:first').addClass('active');
        bindChatEvents();
    });

    if (!window.isMessagesBoxRendered) {
        let chatId = $('#selectedChatId').val();

        getMessagesByChat(chatId === 0 ? userBoxes[1].id : chatId);

        window.isMessagesBoxRendered = true;
    }
}

const getChats = () => {
    window.connection.invoke("GetChats").then((response) => {
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

const getMessagesByChat = (id) => {
    window.connection.invoke("GetMessagesByChat", Number(id)).then((response) => {
        console.log(response);
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

const onPreviousMessages = () => {
    connection.on("previousMessages", (messagesObject) => {
        const selectedChatBox = $('.messages-box > .list-group > .list-group-item[data-chat-id=' + $('#selectedChatId').val() + ']');
        const chatInfo = {
            recieverFullName: $('.username', selectedChatBox).text().trim(),
            reciverProfilePhoto: $('.user-profile-image', selectedChatBox).prop('src')
        }
        renderChatBox(messagesObject, chatInfo);
        
    });
}

const onUserChats = () => {
    window.connection.on("UserChats", (response) => {
        const newChatUsername = window.location.href.split('username=')
            .slice(-1)[0].split('&')[0];
        const existUserInfo = response.map((chatInfo) => {
            return newChatUsername === chatInfo.recieverUserName && chatInfo;
        }).filter(Boolean)[0];
        let chatList = response;

        response.sort(function (a, b) {
            var dateA = new Date(a.lastMessageDate);
            var dateB = new Date(b.lastMessageDate);
            return dateB - dateA;
        });

        if (existUserInfo || !window.location.search.includes('username')) {
            chatList = existUserInfo ? [existUserInfo] : [];

            chatList = chatList.concat(response.map((chatInfo) => {
                return newChatUsername !== chatInfo.recieverUserName && chatInfo;
            }).filter(Boolean));

            renderMessageBoxes(chatList);

            if (chatList.length === 0) {
                $('.spinner').css('visibility', 'hidden');
            }
        } else {
            startNewChat().done((userResponse) => {
                const userInfo = userResponse.data;
                const userBox = {
                    id: 0,
                    recieverFullName: userInfo.firstName + ' ' + userInfo.lastName,
                    lastMessage: '',
                    recieverProfilePhoto: userInfo.profilePhoto,
                    recieverUserName: userInfo.username
                };

                chatList = [userBox].concat(response);

                renderMessageBoxes(chatList);
            });
        }
    });
}

const startNewChat = () => {
    const userId = window.location.search.split('userId=').slice(-1)[0].split('&')[0];

    if (userId) {
        var settings = {
            "url": `${window.config.baseUrl}api/user/${userId}`,
            "method": "GET",
            "timeout": 0,
            "headers": {
                "Authorization": `Bearer ${document.cookie.split('AccessToken=').slice(-1)[0].split(';')[0]}`,
            },
        };

        return $.ajax(settings);
    }

    return {};
}

const sendPrivate = (receiverName, message) => {
    connection.invoke("SendPrivate", receiverName, message).catch(function (err) {
        return console.error(err.toString());
    });
};

const timeDifferenceMessage = (compareDate) => {
    var currentDate = new Date();
    var comparedDate = new Date(compareDate);

    var timeDifference = currentDate - comparedDate;

    var minuteDifference = Math.floor(timeDifference / (1000 * 60));
    var hourDifference = Math.floor(timeDifference / (1000 * 60 * 60));

    if (minuteDifference < 1) {
        return "Şimdi Gönderildi";
    } else if (minuteDifference < 60) {
        return minuteDifference + " dakika önce.";
    } else if (hourDifference < 24) {
        return hourDifference + " saat önce.";
    } else {
        return "Uzun zaman önce";
    }
}

const onNewMessage = () => {
    connection.on("newMessage", (message) => {
        console.log('NEW MESSAGE DEV', message);

        if (isOnChatPage) {
            getMessagesByChat($('#selectedChatId').val());
        } else { 
            const toastTemplate = `
            <input type="hidden" id="createdAt" value="${message.createdAt}"/>
            <div class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="toast-header">
                    <strong class="me-auto">${message.senderFullName}</strong>
                    <small class="text-muted timeDifference">${timeDifferenceMessage(message.createdAt)}</small>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                   ${message.content}
                </div>
            </div>`;

            $('.toast-container').prepend(toastTemplate);
            $('.toast').toast('show');
        }
    });
}

connectionBuild();