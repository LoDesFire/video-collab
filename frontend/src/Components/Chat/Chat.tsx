import React, {useEffect, useRef, useState} from "react";
import {Message} from "./Message";
import {MessageModel} from "./MessageModel";
import {
    AiOutlineLink,
    AiOutlineSend,
    AiOutlineTeam,
    AiOutlineCloseCircle,
    AiOutlineVideoCameraAdd
} from "react-icons/ai";
import {toast} from "react-toastify";
import {MembersElement} from "./MembersElement";
import {MovieDto} from "../../Models/MovieDto";
import {MoviesListElement} from "./MovieListElement";
import {movieAddNew} from "../../Services/MovieService";

type ChatProps = {
    messages: MessageModel[],
    chatMembers: Map<string, string>
    sendMessage: (text: string) => void,
    leaveFromRoom: () => void,
    setOperator: (id: string) => void,
    isOwner: boolean,
    operatorId: string | undefined,
    myId: string,
    movieList: MovieDto[]
}

enum ChatStates {
    Chat,
    Members,
    Movies
}

export const Chat = ({
                         messages,
                         sendMessage,
                         chatMembers,
                         leaveFromRoom,
                         setOperator,
                         isOwner,
                         operatorId,
                         myId,
                         movieList
                     }: ChatProps) => {

    const [input, setInput] = useState<string>('');
    const [showChatMembers, setShowChatMembers] = useState(false)

    const [currentChatState, setCurrentChatState] = useState(ChatStates.Chat)

    const messagesEndRef = useRef<HTMLDivElement>(null);
    const handlePressEnter = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == "Enter")
            handleSend()
    }

    useEffect(() => {
        if (!isOwner && myId != operatorId && currentChatState == ChatStates.Movies) {
            setCurrentChatState(ChatStates.Chat)
        }
    }, [operatorId]);


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
        setCurrentChatState(currentChatState != ChatStates.Members ? ChatStates.Members : ChatStates.Chat)
    }

    const handleShowMovies = () => {
        setCurrentChatState(currentChatState != ChatStates.Movies ? ChatStates.Movies : ChatStates.Chat)
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
            <div className="p-4 bg-gray-100 border-t flex ">
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


    return (
        <div className="flex flex-col h-full">
            {currentChatState == ChatStates.Chat && ChatElement}
            {currentChatState == ChatStates.Members &&
                (
                    <MembersElement
                        chatMembers={chatMembers}
                        isOwner={isOwner}
                        setOperator={setOperator}
                        myId={myId}
                        operatorId={operatorId}/>
                )
            }
            {currentChatState == ChatStates.Movies && <MoviesListElement movieList={movieList}/>}
            <div className="p-4 bg-gray-100 border-t flex ">
                <button
                    onClick={handleCopyLink}
                    className="ml-2 mr-2 p-1 rounded hover:bg-gray-100">
                    <AiOutlineLink size={24}/>
                </button>
                <button
                    onClick={handleShowChatMembers}
                    className={
                        "ml-2 mr-2 p-1 rounded hover:bg-gray-0 " +
                        (currentChatState == ChatStates.Members ? "bg-green-400 rounded" : "")}>
                    <AiOutlineTeam size={24}/>
                </button>
                {(operatorId == myId || isOwner) && (
                    <button
                        onClick={handleShowMovies}
                        className={
                            "ml-2 mr-2 p-1 rounded hover:bg-gray-0 " +
                            (currentChatState == ChatStates.Movies ? "bg-green-400 rounded" : "")}>
                        <AiOutlineVideoCameraAdd size={24}/>
                    </button>
                )
                }
                <button
                    onClick={handleLeavePage}
                    className="ml-2 mr-2 p-1 rounded hover:bg-gray-0">
                    <AiOutlineCloseCircle color="red" size={24}/>
                </button>
            </div>
        </div>
    )

}