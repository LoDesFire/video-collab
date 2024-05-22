import React, {useEffect, useState} from "react";
import {useAuth} from "../../Context/useAuth";
import {deleteRoom, userInfoGetAPI, userPinMovieAPI, userUnpinMovieAPI} from "../../Services/UserService";
import {UserProfileInfo} from "../../Models/User";
import {MovieList} from "../../Components/MovieCard/MovieCardList";
import {toast} from "react-toastify";
import {Link} from "react-router-dom";
import Navbar from "../../Components/Navbar/Navbar";
import {RoomList} from "../../Components/RoomCard/RoomCardList";

type Props = {};

const ProfilePage = (props: Props) => {

    useEffect(() => {
        getUserInfo()
    }, []);

    const {user} = useAuth();
    const [userInfo, setUserInfo] = useState<UserProfileInfo | null>(null)
    const [listOfPinned, setListOfPinned] = useState<Map<number, boolean>>(new Map<number, boolean>)

    const getUserInfo = () => {
        userInfoGetAPI().then(r => {
                if (r?.data) {
                    setUserInfo(r.data)
                    if (r.data?.pinnedMovies != undefined) {
                        for (const a of r.data?.pinnedMovies) {
                            setListOfPinned((it) => {
                                return it?.set(a.id, true)
                            })
                        }
                    }
                }
            }
        ).catch((e) => {
            setUserInfo(null)
        })
    }

    const userPinMovie = (id: number) => {
        userPinMovieAPI(id).then(r => {
            if (r?.status == 200) {
                setListOfPinned((it) => {
                    toast.success("Прикреплено")
                    it?.set(id, true)
                    return it
                })
            }
        }).catch(() => {
        })
    }
    const userUnpinMovie = (id: number) => {
        userUnpinMovieAPI(id).then(r => {
            if (r?.status == 200) {
                setListOfPinned((it) => {
                    toast.success("Откреплено")
                    it?.set(id, false)
                    return it
                })
            }
        })
    }

    const handleDeleteRoom = (id: string) => {
        deleteRoom(id).then(r => {
            if(r?.status == 200) {
                getUserInfo()
                toast.success("Комната удалена")
            }
        })
    }

    return (
        <>
            <Navbar/>
            <div className="container mx-auto w-full my-10">
                <div className="flex w-full justify-between">
                    <h1 className="text-3xl text-left font-bold">
                        Сохраненные фильмы
                    </h1>
                    <Link to="/upload">
                        <button className="px-8 py-3 font-bold rounded text-white bg-green-700 hover:opacity-95">
                            <h1>
                                Загрузить свой фильм
                            </h1>
                        </button>
                    </Link>
                </div>
                <MovieList savedMoviesResult={userInfo?.pinnedMovies ? userInfo.pinnedMovies : []}
                           listOfPinned={listOfPinned}
                           userPinMovie={(id: number) => userPinMovie(id)}
                           userUnpinMovie={(id: number) => userUnpinMovie(id)}
                />
                <div className="flex w-full justify-between">
                    <h1 className="text-3xl text-left font-bold">
                        Комнаты
                    </h1>
                </div>
                <RoomList myRoomsList={userInfo?.ownedRooms ? userInfo.ownedRooms : []} onDelete={handleDeleteRoom}/>
            </div>
        </>
    );
};

export default ProfilePage;
