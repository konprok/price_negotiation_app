# price_negotiation_app
Aplikacja jest testowym zadaniem realizowanym na potrzeby rekrutacji na staż do Software Mind


------------------------------------------------------
1. UŻYTKOWNICY (UserController)
------------------------------------------------------
Kontroler: /users

1.1. POST /users/register
Rejestracja nowego użytkownika.
- Parametry (body JSON):
{
"userName": "string",
"password": "string",
"email": "string"
}
- Odpowiedź: UserResponse (JSON) lub kod 400 w razie błędu
(np. nieprawidłowy format e-mail, istniejący już użytkownik itp.).

       Przykładowe wywołanie (cURL):
       curl -X POST "https://host/users/register" \
            -H "Content-Type: application/json" \
            -d '{
                  "userName": "Bob",
                  "password": "Pass123",
                  "email": "bob@example.com"
                }'

1.2. POST /users/login
Logowanie istniejącego użytkownika.
- Parametry (body JSON):
{
"email": "string",
"password": "string"
}
- Odpowiedź: UserResponse (JSON) lub kod 400 w razie nieprawidłowego
hasła / nieistniejącego konta.

       Przykładowe wywołanie (cURL):
       curl -X POST "https://host/users/login" \
            -H "Content-Type: application/json" \
            -d '{
                  "email": "bob@example.com",
                  "password": "Pass123"
                }'


------------------------------------------------------
2. PRODUKTY (ProductController)
------------------------------------------------------
Kontroler: /products

2.1. POST /products
Dodawanie nowego produktu przez konkretnego użytkownika.
- Parametry:
- Query/string: userId (GUID)
- Body (JSON):
{
"name": "string",
"description": "string",
"basePrice": decimal
}
- Odpowiedź: ProductEntity (JSON) lub kod 400 w razie błędu.

       Przykładowe wywołanie (cURL):
       curl -X POST "https://host/products?userId=00000000-0000-0000-0000-000000000000" \
            -H "Content-Type: application/json" \
            -d '{
                  "name": "Laptop",
                  "description": "Gaming laptop",
                  "basePrice": 2000.00
                }'

2.2. GET /products
Pobranie pojedynczego produktu po parametrze productId.
- Query/string: productId (long)
- Odpowiedź: ProductEntity (JSON) lub kod 400 (jeśli nie znaleziono).

       Przykładowe wywołanie (cURL):
       curl -X GET "https://host/products?productId=123"

2.3. GET /products/{userId}
Pobranie wszystkich produktów danego użytkownika.
- Parametr w ścieżce: {userId} (GUID)
- Odpowiedź: lista ProductEntity lub kod 400 w razie błędu
(np. jeśli user nie istnieje).

       Przykładowe wywołanie (cURL):
       curl -X GET "https://host/products/00000000-0000-0000-0000-000000000000"

2.4. GET /products/all
Pobranie listy wszystkich produktów w bazie.
- Odpowiedź: lista ProductEntity w formacie JSON.

       Przykład (cURL):
       curl -X GET "https://host/products/all"


------------------------------------------------------
3. NEGOCJACJE (NegotiationController)
------------------------------------------------------
Kontroler: /negotiations

3.1. POST /negotiations/proposition
Rozpoczęcie negocjacji lub dodanie nowej propozycji cenowej.
- Parametry (w query/string):
clientId (GUID), productId (long), price (decimal)
- Odpowiedź: PropositionEntity (JSON) lub kod 400 w razie:
* zakończonej negocjacji,
* przekroczenia limitu propozycji,
* upłynięcia czasu na nową propozycję,
* błędów wejściowych itp.

       Przykładowe wywołanie (cURL):
       curl -X POST "https://host/negotiations/proposition?clientId=...&productId=...&price=..."

3.2. PATCH /negotiations/proposition
Akceptacja lub odrzucenie ostatniej propozycji przez właściciela produktu.
- Parametry (w query/string):
userId (GUID), negotiationId (long), response (bool)
np. response=true (akceptacja), response=false (odrzucenie)
- Odpowiedź: aktualna (zaktualizowana) PropositionEntity lub kod 400,
jeśli negocjacja nie istnieje / błąd inny.

       Przykład (cURL):
       curl -X PATCH "https://host/negotiations/proposition?userId=...&negotiationId=10&response=true"

3.3. GET /negotiations
Pobiera wszystkie negocjacje należące do określonego użytkownika (OwnerId).
- Parametr: userId (GUID) w query
- Odpowiedź: lista NegotiationEntity (JSON).

       Przykład (cURL):
       curl -X GET "https://host/negotiations?userId=..."