import React from 'react';
import "react-toastify/dist/ReactToastify.css";
import './App.css';
import {Outlet} from 'react-router-dom';
import {ToastContainer} from "react-toastify";
import {UserProvider} from "./Context/useAuth";

function App() {
    return (
        <>
            <UserProvider>
                <Outlet/>
                <ToastContainer/>
            </UserProvider>
        </>
    );
}

export default App;
