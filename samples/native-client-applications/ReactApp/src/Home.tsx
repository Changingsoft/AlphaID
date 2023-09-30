import React from 'react';

import { useOidc } from './oidc';

export const Home = () => {
    const { login, logout, renewTokens, isAuthenticated } = useOidc();

    return (
        <div className="container-fluid mt-3">
            <div className="card">
                <div className="card-body">
                    <h5 className="card-title">首页</h5>
                    <p className="card-text">这是一个在React中借助react-oidc库调用Alpha ID认证的demo </p>
                    <p className="card-text">react-oidc:<a href="https://github.com/AxaGuilDEv/react-oidc">GitHub @axa-fr/react-oidc</a></p>
                    {!isAuthenticated && <p><button type="button" className="btn btn-primary" onClick={() => login('/profile')}>登录</button></p>}
                    {isAuthenticated && <p><button type="button" className="btn btn-primary" onClick={() => logout('/profile')}>登出 /profile</button></p>}
                    {isAuthenticated && <p><button type="button" className="btn btn-primary" onClick={() => logout()}>logout</button></p>}
                    {isAuthenticated && <p><button type="button" className="btn btn-primary" onClick={() => logout(null)}>logout whithout callbackredirect</button></p>}
                    {isAuthenticated && <p><button type="button" className="btn btn-primary" onClick={async () => console.log('renewTokens result', await renewTokens())}>renew tokens</button></p>}
                </div>
            </div>
        </div>
    );
};
