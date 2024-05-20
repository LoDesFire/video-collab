import {MessageTypesEnum} from "./MessageTypesEnum";

export type MessageModel = {
    text: string,
    from: string,
    type: MessageTypesEnum
}