import { useRecoilState } from 'recoil';

import { history } from '_helpers';
import { authAtom } from '_state';
import { useAlertActions } from '_actions';

export { useFetchWrapper };

function useFetchWrapper() {
    const [auth, setAuth] = useRecoilState(authAtom);
    const alertActions = useAlertActions();

    return {
        get: request('GET'),
        post: request('POST'),
        put: request('PUT'),
        delete: request('DELETE')
    };

    function request(method) {
        return (url, body) => {
            const requestOptions = {
                method,
                headers: authHeader(url)
            };
            if (body) {
                requestOptions.headers['Content-Type'] = 'application/json';
                requestOptions.body = JSON.stringify(body);
            }
            return fetch(url, requestOptions).then(handleResponse);
        }
    }

    function authHeader(url) {
        // возвращаем шапкку с данным посли авторизации успешной
        const token = auth?.token;
        const isLoggedIn = !!token;
        const isApiUrl = url.startsWith(process.env.REACT_APP_API_URL);
        if (isLoggedIn && isApiUrl) {
            return { Authorization: `Bearer ${token}` };
        } else {
            return {};
        }
    }

    function handleResponse(response) {
        return response.text().then(text => {
            const data = text && JSON.parse(text);

            if (!response.ok) {
                if ([401, 403].includes(response.status) && auth?.token) {
                    // происходит логаут в случае 401 403
                    localStorage.removeItem('user');
                    setAuth(null);
                    history.push('/account/login');
                }

                const error = (data && data.message) || response.statusText;
                alertActions.error(error);
                return Promise.reject(error);
            }

            return data;
        });
    }
}