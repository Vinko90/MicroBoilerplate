import jwt_decode from "jwt-decode";
import $ from 'jquery';

var jwtToken;
var refreshToken;

function isNullOrEmpty(value) {
    return typeof value == "undefined" || !value;
}

function doLogin(url, data) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
    }).then(
        function (response) {
            showResponse(response);
            jwtToken = response.access_token;
            refreshToken = response.refresh_token;
            showDecodedToken();
            showDecodedRefreshToken();
        },
        function (xhr, status, error) {
            console.log({ xhr: xhr, status: status, error: error });
            if (xhr.status === 401) {
                alert("Login failed. Please enter a valid username and password");
                jwtToken = "";
                refreshToken = "";
            }
        }
    );
}

function doRefreshToken(url, data) {
    $.ajax({
        headers: {
            "X-XSRF-TOKEN": getCookie("XSRF-TOKEN"), // this is necessary for [AutoValidateAntiforgeryTokenAttribute] and all of the 'POST' requests
            Authorization: "Bearer " + jwtToken, // this is necessary to populate the this.HttpContext.User object automatically
        },
        url: url,
        data: JSON.stringify(data),
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
    }).then(
        function (response) {
            showResponse(response);
            jwtToken = response.access_token;
            refreshToken = response.refresh_token;
            showDecodedToken();
            showDecodedRefreshToken();
        },
        function (xhr, status, error) {
            console.log({ xhr: xhr, status: status, error: error });
            if (xhr.status === 401) {
                alert("Login failed. Please enter a valid username and password");
                jwtToken = "";
                refreshToken = "";
            }
        }
    );
}

function doCallApi(url) {
    $.ajax({
        headers: { Authorization: "Bearer " + jwtToken },
        url: url,
        type: "GET",
    }).then(
        function (response) {
            console.log("response", response);
            if (response) {
                showResponse(response);
            }
        },
        function (xhr, status, error) {
            console.log({ xhr: xhr, status: status, error: error });
            if (xhr.status === 401) {
                alert("Access denied! Please login with an account which has enough permissions first");
            }
        }
    );
}

function doLogout(url) {
    if (isNullOrEmpty(jwtToken)) {
        // will check if the value is undefined or null.
        alert("Please login first.");
        return;
    }

    $.ajax({
        headers: { Authorization: "Bearer " + jwtToken },
        url: url,
        type: "GET",
    }).then(
        function (response) {
            console.log("response", response);
            if (response) {
                showResponse(response);
            }
            jwtToken = "";
            refreshToken = "";
        },
        function (xhr, status, error) {
            console.log({ xhr: xhr, status: status, error: error });
            if (xhr.status === 401) {
                alert("Access denied! Please login with an account which has enough permissions first");
            }
        }
    );
}

function getCookie(name) {
    const value = "; " + document.cookie;
    const parts = value.split("; " + name + "=");
    if (parts.length === 2) {
        const lastItem = parts.pop();
        if (lastItem) {
            const uri = lastItem.split(";").shift();
            if (uri) {
                const cookie = decodeURIComponent(uri);
                console.log("cookie[" + name + "]", cookie);
                return cookie;
            }
        }
    }
    return "";
}

function showDecodedToken() {
    $("#decodedToken").show();
    let decodedToken = jwt_decode(jwtToken);
    console.log(decodedToken);
    $("#jwtInfo").text(JSON.stringify(decodedToken).replace(/,/g, ",\n"));
}

function showDecodedRefreshToken() {
    $("#decodedRefreshToken").show();
    let decodedRefreshToken = jwt_decode(refreshToken);
    console.log(decodedRefreshToken);
    $("#jwtRefreshInfo").text(
        JSON.stringify(decodedRefreshToken).replace(/,/g, ",\n")
    );
}

function showResponse(response) {
    $("#ajaxResponse").show();
    let responseStr = JSON.stringify(response, null, " ");
    console.log("response", response);
    $("#ajaxResponseInfo").text(responseStr);
}