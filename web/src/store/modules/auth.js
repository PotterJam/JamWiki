/* eslint-disable no-unused-vars */
/* eslint-disable promise/param-names */
import { AUTH_REQUEST, AUTH_ERROR, AUTH_SUCCESS, AUTH_LOGOUT } from '../actions/auth'
import axios from 'axios'

const state = { token: localStorage.getItem('user-token') || '', status: '', hasLoadedOnce: false }

const getters = {
  isAuthenticated: state => !!state.token,
  authStatus: state => state.status,
}

const actions = {
  [AUTH_REQUEST]: ({commit, dispatch}, googleIdToken) => {
    return new Promise((resolve, reject) => {
      commit(AUTH_REQUEST)
      axios.post('http://localhost:5000/api/auth', {
        tokenId: googleIdToken
      }).then(resp => {
        const jwtBearer = 'Bearer ' + resp.data.token;
        localStorage.setItem('user-token', jwtBearer);
        axios.defaults.headers.common['Authorization'] = jwtBearer;
        commit(AUTH_SUCCESS, resp);
        resolve(resp);
      }).catch(err => {
        commit(AUTH_ERROR, err)
        localStorage.removeItem('user-token')
        localStorage.setItem('failed', err)
        reject(err)
      });
    });
  },
  [AUTH_LOGOUT]: ({commit, dispatch}) => {
    return new Promise((resolve, reject) => {
      commit(AUTH_LOGOUT)
      localStorage.removeItem('user-token');
      resolve()
    })
  }
}

const mutations = {
  [AUTH_REQUEST]: (state) => {
    state.status = 'loading'
  },
  [AUTH_SUCCESS]: (state, resp) => {
    state.status = 'success'
    state.token = resp.token
    state.hasLoadedOnce = true
  },
  [AUTH_ERROR]: (state) => {
    state.status = 'error'
    state.hasLoadedOnce = true
  },
  [AUTH_LOGOUT]: (state) => {
    state.token = ''
  }
}

export default {
  state,
  getters,
  actions,
  mutations,
}
