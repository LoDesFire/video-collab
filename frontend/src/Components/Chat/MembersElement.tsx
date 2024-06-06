import React from "react";
import {AiFillCamera, AiOutlineCamera} from "react-icons/ai";

type MembersElementProps =
    {
        chatMembers: Map<string, string>
        isOwner: boolean
        setOperator: (id: string) => void
        myId: string,
        operatorId: string | undefined
    }

export const MembersElement = ({chatMembers, isOwner, setOperator, operatorId, myId}: MembersElementProps) => {
    const background = "flex items-center p-2 mt-2.5 mb-2.5 rounded text-white"
    const backgroundMe = background + " bg-green-600"
    const backgroundOther = background + " bg-gray-600"
    return (
        <>
            <h2 className="text-xl font-bold mb-4 pl-4 pt-4">Members</h2>
            <div className="flex flex-col h-full overflow-y-auto p-4">
                <div className="flex flex-col flex-grow">
                    {Array.from(chatMembers.values()).map((member, index) => (
                        <div className={Array.from(chatMembers.keys())[index] == myId ? backgroundMe : backgroundOther}
                             key={index}>
                            <div className="w-12 text-left">{index + 1}.</div>
                            <div className={"flex-1 text-center"}>{member}</div>
                            {isOwner &&
                                (
                                    Array.from(chatMembers.keys())[index] != operatorId ?
                                        (
                                            <button
                                                onClick={() => {
                                                    setOperator(Array.from(chatMembers.keys())[index])
                                                }}
                                                className="text-right">
                                                <AiOutlineCamera size={24}/>
                                            </button>
                                        ) :
                                        (
                                            <div className="text-right">
                                                <AiFillCamera size={24}/>
                                            </div>
                                        )
                                )
                            }
                        </div>
                    ))}
                </div>
            </div>
        </>
    )
}