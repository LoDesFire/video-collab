import {RoomItemDto} from "../../Models/RoomItemDto";
import React from "react";
import {FaTrashAlt} from "react-icons/fa";
import {Link} from "react-router-dom";

interface Props {
    room: RoomItemDto
    onDelete: (id: string) => void
}

export const RoomCard = ({room, onDelete}: Props) => {


    const handleOnDelete = () => {
        onDelete(room.id)
    }

    return (
        <div className="border p-4 rounded-lg shadow-md flex flex-col space-y-4">
            {/* Room ID */}
            <div className="text-xl font-bold">{room.id}</div>

            {/* Members count and buttons */}
            <div className="flex justify-between items-center">
                {/* Members Count */}
                {/*<div className="text-lg">{"" + room.usersCount} members</div>*/}

                {/* Buttons */}
                <div className="flex space-x-2">
                    <div
                        className="bg-green-700 text-white px-4 py-2 rounded hover:bg-green-600 transition"
                    >
                        <Link to={'/connect/' + room.id}>
                            Join
                        </Link>
                    </div>
                    <button
                        onClick={handleOnDelete}
                        className="bg-red-500 text-white p-2 rounded hover:bg-red-600 transition"
                    >
                        <FaTrashAlt/>
                    </button>
                </div>
            </div>
        </div>
    );
}