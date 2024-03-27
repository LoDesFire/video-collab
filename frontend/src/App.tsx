import React from 'react';
import "react-toastify/dist/ReactToastify.css";
import './App.css';
import Navbar from "./Components/Navbar/Navbar";
import {Outlet} from 'react-router-dom';
import {ToastContainer} from "react-toastify";
import {UserProvider} from "./Context/useAuth";

function App() {
    return (
        <>
            <UserProvider>
                <Navbar/>
                <Outlet/>
                <ToastContainer/>
            </UserProvider>
        </>
    );
}

export default App;