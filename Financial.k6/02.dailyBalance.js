import http from 'k6/http';
import { check } from 'k6';

export const options = {
    vus: 1000, 
    duration: '10s', 
};

const authEndpoint = 'http://localhost:44369/Login';
const reportEndpoint = 'http://localhost:44368/api/v1/Report/DailyBalance';
const username = 'master';
const password = 'master';

export default function () {
    const authRes = http.post(authEndpoint, JSON.stringify({ userName: username, password: password }), { headers: { 'Content-Type': 'application/json' } });
    check(authRes, { 'Auth status is 200': (r) => r.status === 200 });
    const authToken = JSON.parse(authRes.body)?.token;

    if (authToken) {
        const reportParams = { headers: { 'Authorization': `Bearer ${authToken}` } };
        const reportRes = http.get(reportEndpoint, reportParams);
        check(reportRes, { 'Report status is 200': (r) => r.status === 200 });
        check(reportRes, { 'Report body is a decimal': (r) => !isNaN(parseFloat(r.body)) && isFinite(r.body) });
        // Opcional: Adicionar verificação do valor do saldo se conhecido.
    }
}