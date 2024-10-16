![logo](./docs/logo.png)


# Messaging: Pragmatycznie (YouTube)
To repozytorium zawiera aplikację Filo, która stanowić będzie oś dyskusji na temat komunikacji asynchronicznej w systemach rozporszonych. Nowe odcinki będą publikowane na **[naszym kanale YouTube.](https://www.youtube.com/@DevMentorsPL)**


# Lista odcinków
1. [Messaging, czyli po co nam komunikacja asynchroniczna?](https://www.youtube.com/watch?v=cA1Cpqk1Zxo)
2. [COUPLING - powiązania i zależności w systemach rozproszonych](https://www.youtube.com/watch?v=q3KOp68QwRA)
3. [ORDERING, czyli dlaczego KOLEJNOŚĆ komunikatów nie jest oczywista?](https://www.youtube.com/watch?v=IXZ_JcGlJVY)
  - przykłady z tego odcinka znajdują się na branchu [ordering](https://github.com/devmentors/Messaging-Pragmatycznie-YouTube/tree/ordering)
4. [PARTYCJONOWANIE, czyli jak zapewnić kolejność przetwarzania wiadomości? | ORDERING cz. 2](https://youtu.be/hcc1fCoK29A)
  - przykłady z tego odcinka znajdują się na branchu [partitioning](https://github.com/devmentors/Messaging-Pragmatycznie-YouTube/tree/partitioning)


## Jak uruchomić?

RabbitMQ uruchamiamy naszybciej poprzez `docker-compose`. Przechodzimy do katalogu `compose`, a następnie w konsoli uruchamiamy polecenie:

```bash
docker-compose up -d
```

Management UI powinien być dostępny pod adrsem `localhost:15672`.


## Tworzenie topologii Filo
Projekt zawiera aplikację konsolową `Filo.Tools.RabbitMqTopology`, która pozwala na szybkie utworzenie topologii w RabbitMQ. Uruchom aplikację, a następnie sprawdź czy została utworzona wymiana (`files-exchange`) wraz z kolejkami dla każdej z aplikacji konsumującej komunikaty.
