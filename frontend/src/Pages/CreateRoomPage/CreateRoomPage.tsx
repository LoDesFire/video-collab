import React from "react";
import {UserService} from "../../Services/UserService";
import {toast} from "react-toastify";
import {useNavigate} from "react-router-dom";
import Navbar from "../../Components/Navbar/Navbar";

const CreateRoomPage = () => {
    const navigate = useNavigate();
    const createRoomBtnClicked = () => {
        UserService.createRoom().then(r => {
                if (r != null) {
                    localStorage.setItem("userOwnerId", r.data.owner.id)
                    localStorage.setItem("roomid", r.data.id)
                    localStorage.setItem("userRoomToken", r.data.owner.textroomToken)
                    navigate('/chat')
                } else {
                    toast.error("Something went wrong")
                }
            }
        )
    }

    return (
        <>
            <Navbar/>
            <section className="bg-white">
                <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto my-20 lg:py-20">
                    <div
                        className="w-full bg-gray-50 rounded-lg shadow md:mb-20 sm:max-w-md xl:p-0">
                        <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                            <h1>Create Room</h1>
                            <button
                                type="submit"
                                className="w-full text-white bg-green-700 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:font-bold"
                                onClick={createRoomBtnClicked}>
                                Create
                            </button>
                        </div>
                    </div>
                </div>
            </section>
        </>
    )
}

export default CreateRoomPage;