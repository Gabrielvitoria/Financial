import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuração para o teste
export const options = {
  vus: 100, // Número de usuários virtuais
  duration: '30s', // Duração do teste
};

const authEndpoint = 'http://localhost:44369/Login'; 
const launchEndpoint = 'http://localhost:44369/Launch'; 
const payEndpoint = 'http://localhost:44369/Pay'; 

// Dados de autenticação
const username = 'master';
const password = 'master';

export default function () {

  // **Passo 1: Autenticação e obtenção do token**
  const authRes = http.post(authEndpoint, JSON.stringify({ userName: username, password: password }), {
    headers: { 'Content-Type': 'application/json' },
  });

  check(authRes, { 'Auth status is 200': (r) => r.status === 200 });
  const authToken = JSON.parse(authRes.body)?.token;

  if (authToken) {
    // **Passo 2: Criação de um novo lançamento**
    const launchPayload = JSON.stringify({
      idempotencyKey: crypto.randomUUID(),
      launchType: 1,
      paymentMethod: 3, 
      coinType: 'BRL',
      value: Math.random() * 100,
      bankAccount: 'Teste K6',
      nameCustomerSupplier: 'K6 Teste',
      costCenter: 'K6',
      description: 'Lançamento para teste de pagamento via K6',
    });
    const launchParams = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${authToken}`,
      },
    };
    const launchRes = http.post(launchEndpoint, launchPayload, launchParams);

    check(launchRes, { 'Launch status is 200': (r) => r.status === 200 });

    // **Passo 3: Extração do ID do lançamento criado**
    const launchId = JSON.parse(launchRes.body)?.id; // Assumindo que a API retorna o ID no campo 'id'

    if (launchId) {
      // **Passo 4: Pagamento do lançamento usando o ID extraído**
      const payPayload = JSON.stringify({
        id: launchId,
        // Adicione outros campos necessários para o /pay, se houver
      });
      const payParams = {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authToken}`,
        },
      };
      const payRes = http.post(payEndpoint, payPayload, payParams);
  
      check(payRes, { 'Pay status is 200': (r) => r.status === 200 });
      check(payRes, { 'Pay response body is not empty': (r) => r.body.length > 0 });
      // console.log(`Launch ${launchId} paid successfully: ${payRes.body}`);
    } else {
      console.error('Falha ao obter o ID do lançamento criado.');
    }
  } else {
    console.error('Falha na autenticação.');
  }
}