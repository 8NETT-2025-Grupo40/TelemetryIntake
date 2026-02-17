# TelemetryIntake API

## Visão Geral

A **TelemetryIntake** é uma API responsável por receber dados de telemetria enviados por sensores instalados nos talhões de uma propriedade rural.

Ela atua como a camada de ingestão dos dados de monitoramento, realizando validações iniciais e publicando os dados em uma fila AWS SQS que será consumida pela API **[FieldMonitoring](https://github.com/8NETT-2025-Grupo40/FieldMonitoring)**.

---

## Contexto de Arquitetura

Fluxo:

Sensores → TelemetryIntake → Amazon SQS → FieldMonitoring

### Descrição do fluxo

1. Os sensores enviam os dados de telemetria para a TelemetryIntake.
2. A API valida e realiza uma análise inicial dos dados.
3. Os dados processados são publicados em uma fila do Amazon SQS.
4. A API FieldMonitoring consome as mensagens da fila.
5. O processamento avançado, persistência, regras de negócio e alertas são completamente gerenciados pela FieldMonitoring.

## Responsabilidades

* Receber dados de sensores de campo
* Validar estrutura e obrigatoriedade dos campos
* Normalizar tipos de dados (ex: string para numérico)
* Aplicar validações básicas de consistência
* Publicar mensagem na fila SQS
* Retornar resposta HTTP adequada ao cliente

## Exemplo de Payload

```json
{
  "SensorId": "cd1ec52e-5677-4c6b-b30e-5e8183ecab59",
  "FarmId": "ebed7141-a8ad-446f-8b08-05a04ceb5b50",
  "FieldId": "627452da-c8c1-41b1-b593-7847e475d504",
  "DateTime": "2026-03-01T11:00:00-03:00",
  "AirTemperature": "25",
  "AirHumidity": "50",
  "SoilTemperature": "25",
  "SoilHumidity": "25",
  "RainMm": "50"
}
```

## Definição dos Campos

| Campo           | Tipo     | Descrição                     |
| --------------- | -------- | ----------------------------- |
| SensorId        | GUID     | Identificador único do sensor |
| FarmId          | GUID     | Identificador da propriedade  |
| FieldId         | GUID     | Identificador do talhão       |
| DateTime        | ISO 8601 | Data e hora da coleta         |
| AirTemperature  | Número   | Temperatura do ar em °C       |
| AirHumidity     | Número   | Umidade relativa do ar (%)    |
| SoilTemperature | Número   | Temperatura do solo em °C     |
| SoilHumidity    | Número   | Umidade do solo (%)           |
| RainMm          | Número   | Volume de chuva em milímetros |

## Endpoint

### POST /telemetry/data

Responsável por receber os dados enviados pelos sensores.

### Headers

Content-Type: application/json  
Accept: \*/\*  
Authorization: Bearer [TOKEN_DE_AUTENTICAÇÃO]

### Possíveis Respostas

204 Status204NoContent  
Dados recebidos e publicados com sucesso na fila.

400 Bad Request  
Payload inválido ou campos obrigatórios ausentes.

401 Unauthorized  
Token não enviado.

403 Forbidden  
Token enviado não possui as permissões corretas.

## Regras de Validação

* Todos os GUIDs devem ser válidos
* DateTime deve estar no formato ISO 8601
* Campos numéricos devem ser convertíveis para número

## Publicação na Fila

Após validação e normalização:

* O payload é serializado em JSON
* É atribuído um identificador de leitura para os valores enviados
* Publicado na fila configurada do Amazon SQS

## Variáveis

| Variável              | Descrição                                    |
| --------------------- | -------------------------------------------- |
| COGNITO_REGION        | Região do Cognito                            |
| COGNITO_USER_POOL_ID  | User pool para o Cognito (Validar token)     |
| COGNITO_M2M_CLIENT_ID | Client id para o Cognito M2M (Validar token) |
| SQS.QueueUrl          | Url da fila SQS                              |
| SQS.Region            | Região da fila SQS                           |

## Segurança

* Comunicação via HTTPS
* Autenticação por meio do Cognito

## Observabilidade

Uma vez que o serviço está rodando no EKS, é possível utilizar o Cloudwatch para encontrar informações de observabilidade.

## Resumo

A TelemetryIntake é a porta para o monitoramento das propriedades e talhões cadastrados.
Ela garante que os dados dos sensores sejam recebidos, validados e encaminhados de forma confiável para processamento posterior, mantendo a arquitetura desacoplada, escalável e resiliente.