import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import "./Navbar.css";

const Navbar = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState(null);
    const [dropdownVisible, setDropdownVisible] = useState(false);
    const [userName, setUserName] = useState(
        localStorage.getItem("userName") || null
    );
    const [showAddressForm, setShowAddressForm] = useState(false);
    const [savedAddresses, setSavedAddresses] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [defaultAddressId, setDefaultAddressId] = useState(
        localStorage.getItem("defaultAddressId") || null
    );
    const dropdownRef = useRef(null);

    const [address, setAddress] = useState({
        area: "",
        city: "",
        landmark: "",
        floor: "",
        type: "Home",
    });

    useEffect(() => {
        const loadUserData = () => {
            try {
                const userData = localStorage.getItem("user");
                const token = localStorage.getItem("token");
                const defaultAddrId = localStorage.getItem("defaultAddressId");

                if (userData && userData !== "undefined" && token) {
                    const storedUser = JSON.parse(userData);
                    setUser(storedUser);
                    setUserName(storedUser.name || storedUser.userName);
                    if (defaultAddrId) {
                        setDefaultAddressId(defaultAddrId);
                    }
                } else {
                    clearUserData();
                }
            } catch (error) {
                console.error("Error parsing user data:", error);
                clearUserData();
            }
        };

        loadUserData();
    }, []);

    const clearUserData = () => {
        localStorage.removeItem("user");
        localStorage.removeItem("token");
        localStorage.removeItem("userName");
        localStorage.removeItem("defaultAddressId");
        setUser(null);
        setUserName(null);
        setDefaultAddressId(null);
    };

    useEffect(() => {
        const fetchData = async () => {
            if (user) {
                await fetchAddresses();
            }
        };

        fetchData();
    }, [user, loading]);

    useEffect(() => {
        function handleClickOutside(event) {
            if (
                dropdownRef.current &&
                !dropdownRef.current.contains(event.target)
            ) {
                setDropdownVisible(false);
            }
        }
        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, []);

    const handleLogout = async () => {
        try {
            await axios.post(
                "http://localhost:5000/api/auth/logout",
                {},
                { withCredentials: true }
            );
            clearUserData();
            navigate("/");
        } catch (error) {
            console.error("Logout failed:", error);
        }
    };

    const handleAddressChange = (e) => {
        const { name, value } = e.target;
        setAddress((prev) => ({ ...prev, [name]: value }));
    };

    const fetchAddresses = async () => {
        const token = localStorage.getItem("token");

        if (!token) {
            setError("Authentication token not found");
            navigate("/login");
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const response = await fetch(`http://localhost:5191/api/address/`, {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(
                    errorData.message || "Failed to fetch addresses"
                );
            }

            const data = await response.json();
            setSavedAddresses(data.data);

            const defaultId = localStorage.getItem("defaultAddressId");
            if (
                defaultId &&
                !data.data.some((addr) => addr._id === defaultId)
            ) {
                localStorage.removeItem("defaultAddressId");
                setDefaultAddressId(null);
            }
        } catch (err) {
            console.error("Fetch address error:", err.message);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteAddress = async (id) => {
        const token = localStorage.getItem("token");
        if (!token) {
            setError("Authentication token not found");
            return;
        }

        try {
            const res = await fetch(`http://localhost:5191/api/address/${id}`, {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!res.ok) {
                const errorData = await res.json();
                throw new Error(errorData.message || "Delete failed");
            }

            if (id === defaultAddressId) {
                localStorage.removeItem("defaultAddressId");
                setDefaultAddressId(null);
            }

            setSavedAddresses((prev) => prev.filter((addr) => addr._id !== id));
        } catch (err) {
            console.error("Delete address error:", err.message);
            setError(err.message);
        }
    };

    const handleSetDefaultAddress = (id) => {
        localStorage.setItem("defaultAddressId", id);
        setDefaultAddressId(id);
    };

    const handleAddressSubmit = async () => {
        const token = localStorage.getItem("token");
        if (!token) {
            setError("Authentication token not found");
            navigate("/login");
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const response = await fetch("http://localhost:5191/api/address", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    ...address,
                    userId: user._id,
                }),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(
                    errorData.message ||
                        `Failed to save address: ${response.status}`
                );
            }

            const data = await response.json();
            console.log("Address saved successfully:", data);

            // If this is the first address, set it as default
            if (savedAddresses.length === 0) {
                localStorage.setItem("defaultAddressId", data.data._id);
                setDefaultAddressId(data.data._id);
            }

            setAddress({
                area: "",
                city: "",
                landmark: "",
                floor: "",
                type: "Home",
            });

            await fetchAddresses();
            setShowAddressForm(false);
        } catch (error) {
            console.error("Error saving address:", error.message);
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    const openAddressForm = () => {
        if (!user || !localStorage.getItem("token")) {
            navigate("/login");
            return;
        }

        setShowAddressForm(true);
        fetchAddresses();
    };

    // Get the default address object
    const getDefaultAddress = () => {
        if (!defaultAddressId) return null;
        return savedAddresses.find((addr) => addr._id === defaultAddressId);
    };

    return (
        <>
            <nav className="navbar">
                <div className="nav-left">
                    <div className="logo">
                        <a href="/">🍔 FoodCart</a>
                    </div>

                    <div className="address-section">
                        <div className="default-address">
                            <p>
                                <strong>Srinagar</strong>
                            </p>
                            <p>
                                <strong>Rangreth</strong>
                            </p>
                        </div>
                    </div>
                </div>

                {user ? (
                    <div className="user-dropdown" ref={dropdownRef}>
                        <button
                            className="user-button"
                            onClick={() => setDropdownVisible(!dropdownVisible)}
                        >
                            {user.name} ▼
                        </button>
                        {dropdownVisible && (
                            <div className="dropdown-menu">
                                <button onClick={() => navigate("/profile")}>
                                    👤 Profile
                                </button>
                                <button onClick={openAddressForm}>
                                    🏠 Address
                                </button>
                                <button
                                    className="logout-btn"
                                    onClick={handleLogout}
                                >
                                    🚪 Logout
                                </button>
                            </div>
                        )}
                    </div>
                ) : userName ? (
                    <button
                        className="signin-btn"
                        onClick={() => navigate("/profile")}
                    >
                        {userName}
                    </button>
                ) : (
                    <button
                        className="signin-btn"
                        onClick={() => navigate("/login")}
                    >
                        Sign In
                    </button>
                )}
            </nav>

            {showAddressForm && (
                <div className="address-modal-overlay">
                    <div className="address-modal-content">
                        <h3>Enter Address</h3>

                        {error && <div className="error-message">{error}</div>}
                        {loading && (
                            <div className="loading-message">Loading...</div>
                        )}

                        <input
                            className="address-input"
                            type="text"
                            name="area"
                            placeholder="Area"
                            value={address.area}
                            onChange={handleAddressChange}
                            disabled={loading}
                        />
                        <input
                            className="address-input"
                            type="text"
                            name="city"
                            placeholder="City"
                            value={address.city}
                            onChange={handleAddressChange}
                            disabled={loading}
                        />
                        <input
                            className="address-input"
                            type="text"
                            name="landmark"
                            placeholder="Landmark"
                            value={address.landmark}
                            onChange={handleAddressChange}
                            disabled={loading}
                        />
                        <input
                            className="address-input"
                            type="text"
                            name="floor"
                            placeholder="Floor"
                            value={address.floor}
                            onChange={handleAddressChange}
                            disabled={loading}
                        />

                        <div className="address-radio-group">
                            <label className="address-radio-label">
                                <input
                                    type="radio"
                                    name="type"
                                    value="Home"
                                    checked={address.type === "Home"}
                                    onChange={handleAddressChange}
                                    disabled={loading}
                                />
                                Home
                            </label>
                            <label className="address-radio-label">
                                <input
                                    type="radio"
                                    name="type"
                                    value="Work"
                                    checked={address.type === "Work"}
                                    onChange={handleAddressChange}
                                    disabled={loading}
                                />
                                Work
                            </label>
                            <label className="address-radio-label">
                                <input
                                    type="radio"
                                    name="type"
                                    value="other"
                                    checked={address.type === "other"}
                                    onChange={handleAddressChange}
                                    disabled={loading}
                                />
                                Other
                            </label>
                        </div>

                        <div className="address-modal-actions">
                            <button
                                className="address-modal-btn address-modal-cancel"
                                onClick={() => setShowAddressForm(false)}
                                disabled={loading}
                            >
                                Cancel
                            </button>
                            <button
                                className="address-modal-btn address-modal-submit"
                                onClick={handleAddressSubmit}
                                disabled={loading}
                            >
                                {loading ? "Saving..." : "Save"}
                            </button>
                        </div>

                        {savedAddresses.length > 0 && (
                            <div className="saved-address-list">
                                <h4>Saved Addresses</h4>
                                <ul>
                                    {savedAddresses.map((addr) => (
                                        <li
                                            key={addr._id}
                                            className="saved-address-item"
                                        >
                                            <div className="address-content">
                                                <div className="address-header">
                                                    {addr.id ===
                                                        defaultAddressId && (
                                                        <span className="default-badge">
                                                            ⭐ DEFAULT
                                                        </span>
                                                    )}
                                                    <strong>
                                                        {addr.addressType ||
                                                            "Other"}{" "}
                                                        Address
                                                    </strong>
                                                </div>

                                                <div className="address-body">
                                                    <p>
                                                        <strong>Area:</strong>{" "}
                                                        {addr.area}
                                                    </p>
                                                    <p>
                                                        <strong>City:</strong>{" "}
                                                        {addr.city}
                                                    </p>
                                                    <p>
                                                        <strong>
                                                            Landmark:
                                                        </strong>{" "}
                                                        {addr.landmark}
                                                    </p>
                                                    <p>
                                                        <strong>Floor:</strong>{" "}
                                                        {addr.floor || "N/A"}
                                                    </p>
                                                    {addr.shopNumber && (
                                                        <p>
                                                            <strong>
                                                                Shop No.:
                                                            </strong>{" "}
                                                            {addr.shopNumber}
                                                        </p>
                                                    )}
                                                </div>
                                            </div>

                                            <div className="address-actions">
                                                {addr.id !==
                                                    defaultAddressId && (
                                                    <button
                                                        onClick={() =>
                                                            handleSetDefaultAddress(
                                                                addr.id
                                                            )
                                                        }
                                                        disabled={loading}
                                                        className="set-default-btn"
                                                    >
                                                        Set Default
                                                    </button>
                                                )}
                                                <button
                                                    onClick={() =>
                                                        handleDeleteAddress(
                                                            addr._id
                                                        )
                                                    }
                                                    disabled={loading}
                                                    className="delete-btn"
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </div>
                </div>
            )}
        </>
    );
};

export default Navbar;
