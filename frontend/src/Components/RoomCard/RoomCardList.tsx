import {RoomItemDto} from "../../Models/RoomItemDto";
import {RoomCard} from "./RoomCard";

interface Props {
    myRoomsList: RoomItemDto[]
    onDelete: (id: string) => void
}

export const RoomList = ({myRoomsList, onDelete}: Props) => {
    return (
        <div className="container mx-auto my-10">
            <div className="grid md:grid-cols-2 sm:grid-cols-1 lg:grid-cols-4 gap-4">
                {
                    myRoomsList.length > 0 ? (
                        myRoomsList.map((room) => {
                            return (
                                <RoomCard
                                    room={room}
                                    onDelete={onDelete}
                                />
                            )
                        }).reverse()
                    ) : (
                        <p className="mb-3 mt-3 text-xl font-semibold text-center md:text-xl">
                            У тебя нет комнат
                        </p>
                    )
                }
            </div>
        </div>
    )
}