import exp from "node:constants";
import {MovieDto} from "./MovieDto";

export type UserProfileToken = {
    id: string,
    username: string,
    token: string,
}

export type UserProfileId = {
    id: string
    username: string,
}

export type UserProfileInfo = {
    pinnedMovies: MovieDto[],
    users: [],
    ownedRooms: []
}