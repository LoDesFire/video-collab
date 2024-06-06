import axios from "axios";
import {CreateRoomInfo, JoinRoomInfo, UserProfileInfo} from "../Models/UserDto";
import {handleError} from "../Helper/ErrorHandler";

export class UserService {
    static async userInfoGetAPI() {
        try {
            return await axios.get<UserProfileInfo>("user/profile")
        } catch (error) {
            handleError(error)
        }
    }

    static async createRoom() {
        try {
            return await axios.post<CreateRoomInfo>("room?private=true",)
        } catch (error) {
            handleError(error)
        }
    }

    static async leaveRoom(roomId: string) {
        try {
            return await axios.get("room/leave?roomId=" + roomId)
        } catch (error) {
            handleError(error)
        }
    }

    static async connectRoom(roomId: string) {
        try {
            return await axios.get<JoinRoomInfo>("room/join?roomId=" + roomId)
        } catch (error) {
            handleError(error)
        }
    }

    static async deleteRoom(roomId: string) {
        try {
            return await axios.delete("room?roomId=" + roomId)
        } catch (error) {
            handleError(error)
        }
    }

    static async userPinMovieAPI(id: number) {
        try {
            return await axios.put("user/pinnedMovies?movieId=" + id)
        } catch (error) {
            handleError(error)
        }
    }

    static async userUnpinMovieAPI(id: number) {
        try {
            return await axios.delete("user/pinnedMovies", {params: {movieId: id}})
        } catch (error) {
            handleError(error)
        }
    }
}