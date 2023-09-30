import React, { useReducer } from 'react';
import { BrowserRouter, NavLink, Route, Routes } from 'react-router-dom';

import { configurationAlphaID } from './configurations';
import { Home } from './Home';
import { OidcProvider, withOidcSecure } from './oidc';
import { Profile, SecureProfile } from './Profile';

const OidcSecureHoc = withOidcSecure(Profile);

const getRandomInt = (max) => {
  return Math.floor(Math.random() * max);
};

function reducer(state, action) {
  switch (action.type) {
    case 'event':
      {
        const id = getRandomInt(9999999999999).toString();
        return [{ ...action.data, id, date: Date.now() }, ...state];
      }
    default:
      throw new Error();
  }
}

function App() {
  const [show, setShow] = React.useState(false);
  const [events, dispatch] = useReducer(reducer, []);

  const onEvent = (configurationName, eventName, data) => {
    dispatch({ type: 'event', data: { name: `oidc:${configurationName}:${eventName}`, data } });
  };
  return (<>

    <OidcProvider configuration={configurationAlphaID} onEvent={onEvent}>
      <BrowserRouter>
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
          <a className="navbar-brand" href="/">Alpha ID Demo</a>
          <button className="navbar-toggler" type="button" onClick={() => setShow(!show)} data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"/>
          </button>
          <div style={show ? { display: 'block' } : { display: 'none' }} className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav">
              <li className="nav-item">
                <NavLink className="nav-link" to="/">Home</NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/profile">Profile</NavLink>
              </li>
            </ul>
          </div>
        </nav>

        <div>
          <Routes>
            <Route path="/" element={<Home></Home>} />
            <Route path="/profile" element={<Profile></Profile>} />
          </Routes>
        </div>

      </BrowserRouter>
    </OidcProvider>
       <div className="container-fluid mt-3">
        <div className="card">
          <div className="card-body" >
            <h5 className="card-title">Default configuration Events</h5>
            <div style={{ overflowX: 'hidden', overflowY: 'scroll', maxHeight: '400px' }}>
              {events.map(e => {
                const date = new Date(e.date);
                const dateFormated = `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
                return <p key={e.id}>{dateFormated} {e.name}: { JSON.stringify(e.data)}</p>;
              })}
            </div>
          </div>
        </div>
      </div></>
  );
}

export default App;
