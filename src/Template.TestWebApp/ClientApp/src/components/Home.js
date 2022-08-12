import React, { Component } from 'react';
import './TestMethods';

export class Home extends Component {
  static displayName = Home.name;
  
  render () {
    return (
      <div className="container">
        <form>
            <label>Username:</label>
            <input type={"text"} placeholder={"Enter Username"} name={"username"} required={true}/>
            <label>Password:</label>
            <input type={"password"} placeholder={"Enter Password"} name={"password"} required={true}/>
            <button type="submit">Login</button>
        </form>
        <div>
            <button type="button" 
                    className="btn btn-warning"             
                    onClick="doRefreshToken('/api/account/RefreshToken',{refreshToken: refreshToken})"
                    id="login">
                Refresh Token
            </button>
            <button
                type="button"
                className="btn btn-info"
                onClick="doCallApi('/api/MyProtectedApi')"
                id="callApi">
                Call Protected API ([Authorize])
            </button>
            <button
                type="button"
                className="btn btn-info"
                onClick="doCallApi('/api/MyProtectedAdminApi')"
                id="callAdminApi">
                Call Protected Admin API [Authorize(Roles = "Admin")]
            </button>
        </div>
          <div>
              <button
                  type="button"
                  className="btn btn-danger"
                  onClick="doLogout('/api/account/logout?refreshToken=' + refreshToken)"
                  name="logout">
                  Logout
              </button>
          </div>
          <div className="alert alert-warning">
              Obtaining new tokens using the refresh_token should happen only if the
              id_token has expired. it is a bad practice to call the endpoint to get a
              new token every time you do an API call.
          </div>
          <div id="ajaxResponse">
              <h2>Response</h2>
              <div className="highlight">
                  <pre><code id="ajaxResponseInfo"></code></pre>
              </div>
          </div>
          <div id="decodedToken">
              <h2>Decoded Access Token</h2>
              <div className="highlight">
                  <pre><code id="jwtInfo"></code></pre>
              </div>
          </div>
          <div id="decodedRefreshToken">
              <h2>Decoded Refresh Token</h2>
              <div className="highlight">
                  <pre><code id="jwtRefreshInfo"></code></pre>
              </div>
          </div>
      </div>
    );
  }
}
