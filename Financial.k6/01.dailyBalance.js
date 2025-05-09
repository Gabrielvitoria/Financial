import http from 'k6/http';
import { check, sleep } from 'k6';


// Configuração para o teste de criação de lançamentos
export const options = {
    vus: 100, // Ajuste conforme necessário para controlar a taxa de criação
    duration: '30s', // Ajuste a duração conforme necessário para criar os 1000 lançamentos
    iterations: 1000, // Garante que 1000 lançamentos sejam tentados criar
};

const authEndpoint = 'http://localhost:44369/Login'; // Ajuste a porta da sua API de autenticação
const launchEndpoint = 'http://localhost:44369/Launch'; // Ajuste a porta da sua API de lançamento

// Dados de autenticação
const username = 'master';
const password = 'master';

export default function () {
    // **Passo 1: Autenticação (executado uma vez por VU)**
    const authRes = http.post(authEndpoint, JSON.stringify({ userName: username, password: password }), {
        headers: { 'Content-Type': 'application/json' },
    });
    check(authRes, { 'Auth status is 200': (r) => r.status === 200 });
    const authToken = JSON.parse(authRes.body)?.token;
    sleep(2); // Pequena pausa para não sobrecarregar a API de criação

    if (authToken) {
        // **Passo 2: Criação de um lançamento**
        const launchPayload = JSON.stringify({
            idempotencyKey: crypto.randomUUID(),
            launchType: Math.floor(Math.random() * 3) + 1, // Pode variar
            paymentMethod: 1,
            coinType: 'BRL',
            value: 1.00,
            bankAccount: `Teste K6 RabbitMQ Criação`,
            nameCustomerSupplier: `K6 RabbitMQ Teste Criação`,
            costCenter: 'K6 RabbitMQ Criação',
            description: `Lançamento para teste RabbitMQ Criação`,
        });

        const launchParams = {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`,
            },
        };

        const launchRes = http.post(launchEndpoint, launchPayload, launchParams);
        check(launchRes, { 'Launch created': (r) => r.status === 200 || r.status === 200 });

        sleep(0.1); // Pequena pausa para não sobrecarregar a API de criação
    } else {
        console.error('Falha na autenticação.');
    }
}