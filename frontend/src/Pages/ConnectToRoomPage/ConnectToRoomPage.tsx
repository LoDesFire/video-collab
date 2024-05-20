import React, {useEffect, useRef} from "react";
import {useForm} from "react-hook-form";
import {connectRoom} from "../../Services/UserService";
import {useNavigate, useParams} from "react-router-dom";
import {toast} from "react-toastify";
import Navbar from "../../Components/Navbar/Navbar";

type Props = {};

type ConnectToRoom = {
    roomId: string
};

const ConnectToRoomPage = (props: Props) => {

    const {
        register,
        handleSubmit,
        formState: {errors}
    } = useForm<ConnectToRoom>()

    const {id} = useParams()
    const ref = useRef<any>();

    useEffect(() => {
        toast.success(id)
    }, []);

    const navigate = useNavigate();

    const handleConnect = (form: ConnectToRoom) => {
        let tmpId = id ? id : form.roomId
        connectRoom(tmpId).then(
            r => {
                if (r != null) {
                    localStorage.setItem("userOwnerId", r.data.id)
                    localStorage.setItem("roomid", tmpId)
                    localStorage.setItem("userRoomToken", r.data.textroomToken)
                    navigate('/test/chat')
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
                            <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl">
                                Присоединиться
                            </h1>
                            <form
                                className="space-y-4 md:space-y-6"
                                onSubmit={handleSubmit(handleConnect)}
                                ref={ref}
                            >
                                {!id && (<>
                                    <label
                                        htmlFor="roomId"
                                        className="block mb-2 text-sm font-medium text-gray-900"
                                    >
                                        Id комнаты
                                    </label>
                                    {errors.roomId ? (
                                        <p className="text-black">{errors.roomId.message}</p>
                                    ) : (
                                        ""
                                    )}
                                    <input
                                        type="text"
                                        id="roomId"
                                        className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5"
                                        placeholder="Id "
                                        {...register("roomId")}
                                    />
                                    {errors.roomId ? (
                                        <p className="text-black">{errors.roomId.message}</p>
                                    ) : (
                                        ""
                                    )}
                                </>)}
                                {/*<label className="relative flex justify-between items-center group p-2 text-xl">*/}
                                {/*    Включить камеру*/}
                                {/*    <input type="checkbox"*/}
                                {/*           className="absolute left-1/2 -translate-x-1/2 w-full h-full peer appearance-none rounded-md"/>*/}
                                {/*    <span*/}
                                {/*        className="w-16 h-10 flex items-center flex-shrink-0 ml-4 p-1 bg-gray-300 rounded-full duration-300 ease-in-out peer-checked:bg-green-700 after:w-8 after:h-8 after:bg-white after:rounded-full after:shadow-md after:duration-300 peer-checked:after:translate-x-6 group-hover:after:translate-x-1"></span>*/}
                                {/*</label>*/}
                                {/*<label className="relative flex justify-between items-center group p-2 text-xl">*/}
                                {/*    Включить микрофон*/}
                                {/*    <input type="checkbox"*/}
                                {/*           className="absolute left-1/2 -translate-x-1/2 w-full h-full peer appearance-none rounded-md"/>*/}
                                {/*    <span*/}
                                {/*        className="w-16 h-10 flex items-center flex-shrink-0 ml-4 p-1 bg-gray-300 rounded-full duration-300 ease-in-out peer-checked:bg-green-700 after:w-8 after:h-8 after:bg-white after:rounded-full after:shadow-md after:duration-300 peer-checked:after:translate-x-6 group-hover:after:translate-x-1"></span>*/}
                                {/*</label>*/}
                                <button
                                    type="submit"
                                    className="w-full text-white bg-green-700 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:font-bold"
                                >
                                    Join
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </section>
        </>
    )
}

export default ConnectToRoomPage;