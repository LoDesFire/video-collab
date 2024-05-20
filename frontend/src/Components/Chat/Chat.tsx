import React, {useEffect, useRef, useState} from "react";
import {Message} from "./Message";
import {MessageModel} from "./MessageModel";
import {AiOutlineLink, AiOutlineSend, AiOutlineTeam, AiOutlineCloseCircle} from "react-icons/ai";
import {handleError} from "../../Helper/ErrorHandler";
import {toast} from "react-toastify";
import {wait} from "@testing-library/user-event/dist/utils";
import {leaveRoom} from "../../Services/UserService";
import {useNavigate} from "react-router-dom";

type ChatProps = {
    messages: MessageModel[],
    chatMembers: Map<string, string>
    sendMessage: (text: string) => void,
    leaveFromRoom: () => void,
}

export const Chat = ({messages, sendMessage, chatMembers, leaveFromRoom}: ChatProps) => {

    const [input, setInput] = useState<string>('');
    const [showChatMembers, setShowChatMembers] = useState(false)

    const messagesEndRef = useRef<HTMLDivElement>(null);
    const handlePressEnter = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == "Enter")
            handleSend()
    }


    const handleSend = () => {
        if (input === "") {
            toast.info("Введите текст сообщения")
        } else {
            sendMessage(input)
            setInput("")
        }
    }

    const handleLeavePage = () => {
        leaveFromRoom()
    }

    const handleCopyLink = () => {
        const link = `${window.location.origin}/connect/${localStorage.getItem("roomid")}`
        navigator.clipboard.writeText(link).then(() => {
            toast.info('Link copied to clipboard!');
        });
    }

    const handleShowChatMembers = () => {
        setShowChatMembers(!showChatMembers)
    }

    useEffect(() => {
        if (messages.length) {
            messagesEndRef?.current?.scrollIntoView({
                behavior: "auto",
                block: "end"
            });
        }
    }, [messages.length, showChatMembers]);
    const ChatElement =
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Chat</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow justify-end">
                    {messages.map((message, index) => (
                        <Message key={index} text={message.text} messageType={message.type} from={message.from}/>
                    ))}
                    <div ref={messagesEndRef}/>
                </div>
            </div>
            <div className="p-4 bg-white border-t flex ">
                <input
                    type="text"
                    className="flex-grow p-2 border rounded-l"
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyDown={handlePressEnter}
                />
                <button
                    onClick={handleSend}
                    className="p-2 bg-green-700 text-white rounded-r hover:bg-green-800 flex items-center justify-center"
                >
                    <AiOutlineSend size={24}/>
                </button>
            </div>
        </>

    const MembersElement =
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Members</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow">
                    {Array.from(chatMembers.values()).map((member, index) => (
                        <div className="flex items-center p-2 mt-2.5 mb-2.5 bg-gray-600 rounded text-white" key={index}>
                            <div className="w-12 text-left">{index + 1}.</div>
                            <div className="flex-1 text-center">{member}</div>
                        </div>
                    ))}
                    <div ref={messagesEndRef}/>
                </div>
            </div>
        </>


    return (
        <div className="flex flex-col h-full">
            {!showChatMembers ? ChatElement : MembersElement}
            <div className="p-4 bg-white border-t flex ">
                <button
                    onClick={handleCopyLink}
                    className="ml-2 mr-2 p-1 rounded hover:bg-gray-100">
                    <AiOutlineLink size={24}/>
                </button>
                <button
                    onClick={handleShowChatMembers}
                    className={
                        "ml-2 mr-2 p-1 rounded hover:bg-gray-100 " +
                        (showChatMembers ? "bg-green-400 rounded" : "")}>
                    <AiOutlineTeam size={24}/>
                </button>
                <button
                    onClick={handleLeavePage}
                    className="ml-2 mr-2 p-1 rounded hover:bg-gray-100">
                    <AiOutlineCloseCircle color="red" size={24}/>
                </button>
            </div>
        </div>
    )

}