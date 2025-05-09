import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 1000,
  duration: '10s',
};

export default function () {
sleep(2);
  // Payload da requisição de autenticação
  const payload = JSON.stringify({
    userName: "master",
    password: "master",
  });
  // Headers da requisição
  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  // Faz a requisição POST para o endpoint de autenticação
  let res;
  res = http.post("http://localhost:44369/login", payload, params);
  //const res = http.get('https://quickpizza.grafana.com/');
  check(res, { 'status was 200': (r) => r.status == 200 });
  sleep(2);
}
