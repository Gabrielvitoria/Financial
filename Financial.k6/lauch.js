import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuração para o teste
export const options = {
  vus: 1000, // Número de usuários virtuais (simultâneos)
  duration: '10s', // Duração do teste
};

// URLs dos endpoints via YARP
const authEndpoint = 'http://localhost:44369/login';
const launchEndpoint = 'http://localhost:44369/launch';

// Dados de autenticação
const username = 'master';
const password = 'master';

export default function () {
    sleep(2);
  // **Passo 1: Autenticação e obtenção do token**
  const authPayload = JSON.stringify({
    userName: username,
    password: password,
  });
  const authParams = {
    headers: {
      'Content-Type': 'application/json',
    },
  };
  const authRes = http.post(authEndpoint, authPayload, authParams);
sleep(2);
  check(authRes, {
    'Auth status is 200': (r) => r.status === 200,
    'Auth response contains token': (r) => r.body.includes('token'),
  });

  // Extrai o token do corpo da resposta JSON
  const authToken = JSON.parse(authRes.body)?.token;

  // Verifica se o token foi obtido com sucesso antes de prosseguir
  if (authToken) {
    console.log(`Token obtido: ${authToken}`);

    // **Passo 2: Criação de um novo lançamento usando o token obtido**
    const launchPayload = JSON.stringify({
      idempotencyKey: crypto.randomUUID(),
      launchType: 1,
      paymentMethod: 1,
      coinType: 'BRL',
      value: Math.random() * 100,
      bankAccount: '12345-6',
      nameCustomerSupplier: 'Cliente Teste',
      costCenter: '789',
      description: 'Lançamento de teste via K6 com token dinâmico',
    });

    const launchParams = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authToken}`,
      },
    };
sleep(2);
    const launchRes = http.post(launchEndpoint, launchPayload, launchParams);
sleep(2);
    check(launchRes, {
      'Launch status is 200': (r) => r.status === 200,
      'Launch response body is not empty': (r) => r.body.length > 0,
    });
sleep(2);
    // Log da resposta do lançamento (opcional)
    // console.log(`Launch Response body: ${launchRes.body}`);
  } else {
    console.error('Falha ao obter o token JWT. Pulando o teste de lançamento.');
  }
}