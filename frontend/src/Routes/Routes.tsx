import {createBrowserRouter} from "react-router-dom";
import React from "react";
import App from "../App";
import HomePage from "../Pages/HomePage/HomePage";
import LoginPage from "../Pages/LoginPage/LoginPage";
import RegisterPage from "../Pages/RegisterPage/RegisterPage";
import {AuthRequiredProtectRoute, NotAuthRequiredProtectRoute} from "./AuthRequiredProtectRoute";
import ProfilePage from "../Pages/ProfilePage/ProfilePage";

export const router = createBrowserRouter([
        {
            path: "/",
            element: <App/>,
            children: [
                {
                    path: "",
                    element:
                        <AuthRequiredProtectRoute>
                            <HomePage/>
                        </AuthRequiredProtectRoute>
                },
                {
                    path: "login",
                    element:
                        <NotAuthRequiredProtectRoute>
                            <LoginPage/>
                        </NotAuthRequiredProtectRoute>
                },
                {
                    path: "register",
                    element:
                        <NotAuthRequiredProtectRoute>
                            <RegisterPage/>
                        </NotAuthRequiredProtectRoute>
                },
                {
                    path: "profile",
                    element:
                        <AuthRequiredProtectRoute>
                            <ProfilePage/>
                        </AuthRequiredProtectRoute>
                }
            ]
        }
    ]
)