import React, {useEffect, useState} from "react";
import {UserService} from "../../Services/UserService";
import {UserProfileInfo} from "../../Models/UserDto";
import {MovieList} from "../../Components/MovieCard/MovieCardList";
import {toast} from "react-toastify";
import {Link} from "react-router-dom";
import Navbar from "../../Components/Navbar/Navbar";
import {RoomList} from "../../Components/RoomCard/RoomCardList";
import {MovieDto} from "../../Models/MovieDto";
import {MovieService} from "../../Services/MovieService";

const ProfilePage = () => {

    useEffect(() => {
        getUserInfo()
        getAllMovies()
    }, []);

    const [userInfo, setUserInfo] = useState<UserProfileInfo | null>(null)
    const [allMovies, setAllMovies] = useState<MovieDto[]>([])
    const [listOfPinned, setListOfPinned] = useState<Map<number, boolean>>(new Map<number, boolean>)

    const getUserInfo = () => {
        UserService.userInfoGetAPI().then(r => {
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
        ).catch(() => {
            setUserInfo(null)
        })
    }

    const getAllMovies = () => {
        MovieService.getAllMovie().then(r => {
            if (r?.status == 200) {
                setAllMovies(r.data)
            }
        }).catch(() => {
        })
    }

    const userPinMovie = (id: number) => {
        UserService.userPinMovieAPI(id).then(r => {
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
        UserService.userUnpinMovieAPI(id).then(r => {
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
        UserService.deleteRoom(id).then(r => {
            if (r?.status == 200) {
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
                <div className="flex w-full justify-between">
                    <h1 className="text-3xl text-left font-bold">
                        Все фильмы
                    </h1>
                </div>
                <MovieList savedMoviesResult={allMovies}
                           listOfPinned={listOfPinned}
                           userPinMovie={(id: number) => userPinMovie(id)}
                           userUnpinMovie={(id: number) => userUnpinMovie(id)}
                />

            </div>
        </>
    );
};

export default ProfilePage;
