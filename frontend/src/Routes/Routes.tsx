import {createBrowserRouter} from "react-router-dom";
import React from "react";
import App from "../App";
import HomePage from "../Pages/HomePage/HomePage";
import LoginPage from "../Pages/LoginPage/LoginPage";
import RegisterPage from "../Pages/RegisterPage/RegisterPage";
import {AuthRequiredProtectRoute,} from "./AuthRequiredProtectRoute";
import ProfilePage from "../Pages/ProfilePage/ProfilePage";
import ConnectToRoomPage from "../Pages/ConnectToRoomPage/ConnectToRoomPage";
import CreateRoomPage from "../Pages/CreateRoomPage/CreateRoomPage";
import {UploadVideoPage} from "../Pages/UploadVideoPage/UploadVideoPage";
import {ChatPage} from "../Pages/ChatPage/ChatPage";
import {VideoInfo} from "../Pages/VideoInfoPage/VideoInfo";

export const router = createBrowserRouter([
        {
            path: "/",
            element: <App/>,
            children: [
                {
                    path: "",
                    element: <HomePage/>
                },
                {
                    path: "login",
                    element:
                    /*<NotAuthRequiredProtectRoute>*/
                        <LoginPage/>
                    /*</NotAuthRequiredProtectRoute>*/
                },
                {
                    path: "register",
                    element:
                    /*<NotAuthRequiredProtectRoute>*/
                        <RegisterPage/>
                    /* </NotAuthRequiredProtectRoute>*/
                },
                {
                    path: "profile",
                    element:
                        <AuthRequiredProtectRoute>
                            <ProfilePage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "connect/:id",
                    element:
                        <AuthRequiredProtectRoute>
                            <ConnectToRoomPage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "connect",
                    element:
                        <AuthRequiredProtectRoute>
                            <ConnectToRoomPage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "create",
                    element:
                        <AuthRequiredProtectRoute>
                            <CreateRoomPage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "upload",
                    element:
                        <AuthRequiredProtectRoute>
                            <UploadVideoPage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "movie/:id",
                    element:
                        <AuthRequiredProtectRoute>
                            <VideoInfo/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "chat",
                    element:
                        <AuthRequiredProtectRoute>
                            <ChatPage/>
                        </AuthRequiredProtectRoute>
                }
            ]
        }
    ]
)