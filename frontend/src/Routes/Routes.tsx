import {createBrowserRouter} from "react-router-dom";
import React from "react";
import App from "../App";
import HomePage from "../Pages/HomePage/HomePage";
import LoginPage from "../Pages/LoginPage/LoginPage";
import RegisterPage from "../Pages/RegisterPage/RegisterPage";
import {ProtectedRoute} from "./ProtectedRoute";

export const router = createBrowserRouter([
        {
            path: "/",
            element: <App/>,
            children: [
                {path: "", element: <ProtectedRoute><HomePage/></ProtectedRoute>},
                {path: "login", element: <LoginPage/>},
                {path: "register", element: <RegisterPage/>}
            ]
        }
    ]
)