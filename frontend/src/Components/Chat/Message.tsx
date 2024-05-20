import {MessageTypesEnum} from "./MessageTypesEnum";

type MessageProps = {
    text: string
    from: string
    messageType: MessageTypesEnum
};

export const Message = ({text, from, messageType}: MessageProps) => {

    const StyleForJoinLest = "text-center rounded p-1 "
    const StyleJoin = StyleForJoinLest + "bg-green-600 text-white "
    const StyleLeave = StyleForJoinLest + "bg-red-500 text-white "

    return (
        <div className="p-2 mb-0 pl-2">
            {
                messageType == MessageTypesEnum.join &&
                (
                    <div className={StyleJoin}>
                        {from} join
                    </div>
                )
            }
            {
                messageType == MessageTypesEnum.message &&
                (
                    <div
                        className="bg-gray-100 rounded break-words pl-2">
                        <h6 className="text-green-700">{from} </h6>
                        <h1 className="text-gray-700 text-lg">{text}</h1>
                    </div>
                )
            }
            {
                messageType == MessageTypesEnum.leave &&
                (
                    <div className={StyleLeave}>
                        {from} leave
                    </div>
                )
            }
        </div>
    )
}