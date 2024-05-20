import axios from "axios";
import {CreateRoomInfo, JoinRoomInfo, UserProfileInfo} from "../Models/User";
import {handleError} from "../Helper/ErrorHandler";
import exp from "node:constants";


export const userInfoGetAPI = async () => {
    try {
        return await axios.get<UserProfileInfo>("user/profile")
    } catch (error) {
        handleError(error)
    }
}

export const createRoom = async () => {
    try {
        return await axios.post<CreateRoomInfo>("room", {private: true})
    } catch (error) {
        handleError(error)
    }
}

export const leaveRoom = async (roomId: string) => {
    try {
        return await axios.get("room/leave?roomId=" + roomId)
    } catch (error) {
        handleError(error)
    }
}

export const connectRoom = async (roomId: string) => {
    try {
        return await axios.get<JoinRoomInfo>("room/join?roomId=" + roomId)
    } catch (error) {
        handleError(error)
    }
}

export const userPinMovieAPI = async (id: number) => {
    try {
        return await axios.put("user/pinnedMovies?movieId=" + id)
    } catch (error) {
        handleError(error)
    }
}

export const userUnpinMovieAPI = async (id: number) => {
    try {
        return await axios.delete("user/pinnedMovies", {params: {movieId: id}})
    } catch (error) {
        handleError(error)
    }
}