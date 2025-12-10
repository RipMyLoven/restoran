# Restoran - Restorani Haldussüsteem

## 1. TEOREETILINE OSA

Käesolev peatükk käsitleb projekti aluseks olevaid tehnoloogiaid ja teoreetilisi aspekte. Restoranirakenduse arendamisel kasutatud tehnoloogiad on valitud nende jõudluse, turvalisuse ja kaasaegsuse alusel. Iga tehnoloogia mängib olulist rolli lõpliku süsteemi toimimises ja tagab kasutajatele optimaalse kogemuse.

### 1.1 ASP.NET Core 10.0

ASP.NET Core 10.0 on Microsofti poolt arendatud moodne, avatud lähtekoodiga raamistik veebirakenduste ja API-de loomiseks. See on platvormideülene lahendus, mis töötab Windows, Linux ja macOS operatsioonisüsteemides.

Peamised eelised hõlmavad kõrget jõudlust ja skaleeritavust, sisseehitatud sõltuvuste süstimist (Dependency Injection), middleware torustikku HTTP päringute töötlemiseks. Samuti pakub see sisseehitatud tuge API dokumentatsioonile ja testidele, tugevat turvalisuse süsteemi autentimise ja autoriseerimisega ning Entity Framework Core integratsiooni andmebaasi tööks.

ASP.NET Core 10.0 kasutab minimaalseid API-sid, mis võimaldavad kiiresti luua REST API endpoint'e vähem koodiga. See on ideaalne valik moodsate veebirakenduste backend'i arendamiseks.

### 1.2 MySQL

MySQL on üks populaarsemaid avatud lähtekoodiga relatsiooniandmebaase maailmas. See on tuntud oma kiiruse, usaldusväärsuse ja kasutamise lihtsuse poolest.

MySQL-i peamised tugevused on kõrge jõudlus OLTP rakenduste jaoks, ACID vastavus, skaleeritavus ja replikatsioon. Andmebaas pakub tugevat turvalisust krüpteerimise ja kasutajate haldusega, omab laiat ökosüsteemi ja kogukonnatud ning toetab erinevaid andmetüüpe nagu JSON, XML ja geospatiaalsed andmed.

Restoranirakenduse kontekstis on MySQL suurepärane valik, kuna see suudab tõhusalt hallata tellimusi, menüüelemente, kasutajaid ja muid ärikraalilisi andmeid.

### 1.3 React.js

React.js on Meta (endine Facebook) poolt loodud JavaScript'i raamistik kasutajaliideste ehitamiseks. See on komponentidel põhinev raamistik, mis kasutab virtuaalset DOM'i efektiivseks renderdamiseks.

React'i põhilised kontseptsioonid hõlmavad komponentide arhitektuuri taaskasutatavate UI elementidega, JSX süntaksit HTML'i ja JavaScript'i kombineerimiseks ning state management'i komponendi sisemise oleku halduseks. Samuti kasutab React props'e andmete edastamiseks komponentide vahel, hooks'e funktsionaalsete komponentide halduseks ja Virtual DOM'i efektiivseks renderdamiseks.

React on ideaalne valik dünaamiliste kasutajaliideste loomiseks, kus andmed muutuvad reaalajas, nagu näiteks restorani tellimusesüsteemis.

### 1.4 Microsoft.AspNetCore.Authentication.JwtBearer

JWT (JSON Web Token) Bearer autentimine on moodne ja turvaline viis kasutajate autentimiseks veebirakendustes. Microsoft.AspNetCore.Authentication.JwtBearer paketi abil saab hõlpsalt implementeerida JWT-põhist autentimist ASP.NET Core rakendustes.

JWT-de põhilised eelised on nende stateless olemus (server ei pea sessiooni infot salvestama), kompaktne suurus HTTP headerite jaoks, self-contained struktuur kogu vajaliku informatsiooniga ning cross-domain tugi. JWT on avatud standard (RFC 7519), mis tagab ühilduvuse erinevate süsteemide vahel.

JWT koosneb kolmest osast:
1. Header - algoritm ja tüüp
2. Payload - kasutaja andmed ja claims
3. Signature - digitaalne allkiri turvalisuse tagamiseks

### 1.5 Tailwind CSS

Tailwind CSS on utility-first CSS raamistik, mis võimaldab kiiresti ehitada kohandatud kasutajaliidesi. Erinevalt traditsioonilistest CSS raamistikest, pakub Tailwind madala taseme utility klasse, mis vastavad üksikutele CSS omadustele.

Tailwind CSS-i peamised eelised on kiire arendus utility klasside abil, väike lõplik bundle suurus purge funktsiooniga ning konsistentne disainsüsteem. Raamistik pakub suurepärast responsive design tuge, kohandatavust konfiguratsioonifaili kaudu ja IntelliSense tuge IDE-des.

Utility-first lähenemine tähendab, et stiilid kirjutatakse otse HTML klasside kaudu, näiteks `bg-blue-500 text-white px-4 py-2`, mis teeb arenduse kiiremaks ja hooldatavamaks.

### 1.6 Bun

Bun on uus ja kiire JavaScript runtime, paketi haldur ja bundler, mis on kirjutatud Zig programmeerimiskeeles. See on loodud Node.js-i alternatiivina, pakkudes märkimisväärselt kiiremat jõudlust.

Bun'i peamised eelised on ekstreemselt kiire käivitamine ja käitamine, sisseehitatud bundler ja transpiler ning TypeScript'i native tugi. Runtime pakub Web API-de implementatsiooni, Node.js ökosüsteemi ühilduvust ja väikest mälukasutust.

Bun sisaldab:
- `bun run` - skriptide käivitamine
- `bun install` - sõltuvuste installimine
- `bun build` - rakenduse kokkupanek
- `bun test` - testide käivitamine

### 1.7 Swashbuckle.AspNetCore

Swashbuckle.AspNetCore on .NET-i raamistik, mis genereerib automaatselt OpenAPI (Swagger) dokumentatsiooni ASP.NET Core API-dele. See pakub interaktiivset API dokumentatsiooni, mis on arendajatele ja testimisele väga kasulik.

Swashbuckle pakub automaatset API dokumentatsiooni genereerimist, Swagger UI interaktiivset liidest ning JSON/YAML vormingus OpenAPI spetsifikatsiooni. Lisaks toetab see annotatsioonilist API dokumenteerimist, kohandatavat välimust ja sisu ning API versiooni haldust.

Swagger UI võimaldab API endpoint'ide sirvimist, päringute testimist otse brauseris, skeemade ja mudelite vaatamist ning autentimise testimist.

## 2. PRAKTILINE OSA

### 2.1 Tehnoloogiad ja Tööriistad

Käesolev restoranirakenduse projekt kasutab kaasaegset tehnoloogiate komplekti, mis tagab kõrge jõudluse ja hea kasutajakogemuse.

**Backend tehnoloogiad:**
- ASP.NET Core 10.0 - REST API ja äriloogika
- Entity Framework Core - ORM andmebaasi tööks
- MySQL - andmebaas andmete salvestamiseks
- JWT Bearer Authentication - kasutajate autentimine
- Swagger/OpenAPI - API dokumentatsioon

**Frontend tehnoloogiad:**
- React.js 18 - kasutajaliides
- TypeScript - tüübiturvalisus
- Tailwind CSS - stiilimine
- Bun - runtime ja paketi haldur

**Arendustööriistad:**
- Visual Studio Code - koodiredaktor
- Git - versioonihaldus
- Entity Framework Migrations - andmebaasi skeem

### 2.2 Projekti Struktuur

Projekt koosneb kahest põhiosast: backend (ASP.NET Core) ja frontend (React).

**Backend struktuur:**
```
Controllers/ - API kontrollerid
├── AuthController.cs - autentimine
├── RestaurantsController.cs - restoranide haldus
├── MenuItemsController.cs - menüü haldus
├── OrdersController.cs - tellimused
├── TablesController.cs - laudade haldus
└── BillsController.cs - arvete haldus

Models/ - andmemudelid
├── User.cs - kasutaja
├── Restaurant.cs - restoran
├── MenuItem.cs - menüüelement
├── Order.cs - tellimus
└── Table.cs - laud

DTOs/ - andmeülekande objektid
Data/ - andmebaasi kontekst
Migrations/ - andmebaasi migratsioonid
```

**Frontend struktuur:**
```
src/
├── components/ - taaskasutatavad komponendid
├── pages/ - lehekülgede komponendid
├── api/ - API kliendi kood
├── context/ - React kontekst
├── types/ - TypeScript tüübid
└── layouts/ - lehekülgede paigutused
```

### 2.3 Andmebaasi Struktuur

Andmebaas koosneb järgmistest põhitabelitest:

**Users** - kasutajad
- Id, Name, Email, PasswordHash, Role, CreatedAt

**Restaurants** - restoranid
- Id, Name, Address, Phone, Email, OwnerId, CreatedAt

**Tables** - lauad
- Id, Number, Capacity, RestaurantId, IsOccupied

**MenuItems** - menüüelemendid
- Id, Name, Description, Price, Category, RestaurantId, IsAvailable

**Orders** - tellimused
- Id, UserId, RestaurantId, TableId, Status, TotalAmount, CreatedAt

**OrderItems** - tellimuse elemendid
- Id, OrderId, MenuItemId, Quantity, Price

**Bills** - arved
- Id, OrderId, Amount, PaymentMethod, IsPaid, CreatedAt

**Notifications** - teavitused
- Id, UserId, Message, Type, IsRead, CreatedAt

### 2.4 API Endpoint'id

**Autentimine:**
- POST /api/auth/login - sisselogimine
- POST /api/auth/register - registreerimine
- POST /api/auth/refresh - tokeni uuendamine

**Restoranid:**
- GET /api/restaurants - kõik restoranid
- GET /api/restaurants/{id} - konkreetne restoran
- POST /api/restaurants - uue restorani lisamine
- PUT /api/restaurants/{id} - restorani uuendamine

**Menüü:**
- GET /api/menuitems/restaurant/{id} - restorani menüü
- POST /api/menuitems - uue toidu lisamine
- PUT /api/menuitems/{id} - toidu uuendamine
- DELETE /api/menuitems/{id} - toidu kustutamine

**Tellimused:**
- GET /api/orders - kasutaja tellimused
- POST /api/orders - uue tellimuse loomine
- PUT /api/orders/{id}/status - tellimuse staatuse uuendamine
- GET /api/orders/restaurant/{id} - restorani tellimused

**Lauad:**
- GET /api/tables/restaurant/{id} - restorani lauad
- POST /api/tables - uue laua lisamine
- PUT /api/tables/{id}/status - laua staatuse uuendamine

### 2.5 Funktsioonide Kirjeldus (Frontend)

**Kasutajaliides koosneb järgmistest peamistest funktsioonidest:**

**Autentimine ja kasutajahaldus:**
- Sisselogimise vorm JWT tokeniga
- Registreerimise vorm
- Kasutaja profiili haldus
- Automaatne token uuendamine

**Restorani vaade:**
- Restoranide loendi kuvamine
- Restorani detailvaade menüüga
- Otsingufunktsioon restoranide leidmiseks
- Filtreerimine kategooriate järgi

**Tellimusesüsteem:**
- Menüüelementide sirvimine
- Ostukorvi funktsioonid
- Tellimuse vormistamine
- Tellimuse staatuse jälgimine
- Tellimuse ajalugu

**Laudade haldus:**
- Vabade laudade kuvamine
- Laua broneerimine
- Laua staatuse jälgimine

**Teavitused:**
- Reaalajas teavitused tellimuste kohta
- Teavituste loendi haldus
- Lugemise märgistamine

### 2.6 Funktsioonide Kirjeldus (Backend)

**API kontrollerite funktsioonid:**

**AuthController:**
- Kasutaja autentimine ja JWT tokeni genereerimine
- Parooli hasheerimine ja kontrollimine
- Rolli-põhine autoriseerimine
- Token'i uuendamise loogika

**RestaurantsController:**
- CRUD operatsioonid restoranidega
- Filtreerimine ja otsingufunktsioonid
- Autoriiseeritud juurdepääsu kontroll
- Andmete validatsioon

**MenuItemsController:**
- Menüüelementide haldus
- Kategooriate filtreerimine
- Kättesaadavuse kontrollimine
- Hindade haldus

**OrdersController:**
- Tellimuste loomine ja haldus
- Staatuste uuendamine
- Arvutused summat
- Tellimuste ajaloo pärimine

**TablesController:**
- Laudade haldus ja broneeringud
- Staatuse jälgimine (vaba/kinni)
- Mahutavuse kontrollimine

**BillsController:**
- Arvete genereerimine
- Maksmise staatuse haldus
- Maksemeetodite tugi

## 3. KASUTUSJUHEND

### 3.1 Swagger

Swagger UI on kättesaadav arendusrežiimis järgmisel aadressil: `https://localhost:5001/swagger`

**Swagger kasutamine:**

1. **API dokumentatsiooni sirvimine:**
   - Avage brauseris Swagger UI
   - Näete kõiki saadaolevaid endpoint'e kategooriate kaupa
   - Iga endpoint näitab HTTP meetodit, URL'i ja parameetreid

2. **API päringute testimine:**
   - Klõpsake endpoint'i lahtivoldikuks
   - Täitke vajalikud parameetrid
   - Klõpsake "Try it out" nupul
   - Näete päring ja vastuse detaile

3. **Autentimise testimine:**
   - Kasutage `/api/auth/login` endpoint'i sisselogimiseks
   - Kopeerige tagastatud JWT token
   - Klõpsake "Authorize" nupul lehekülje ülaosas
   - Sisestage token formaadis: `Bearer {token}`
   - Nüüd saate testida kaitstud endpoint'e

4. **Mudeli skeemade vaatamine:**
   - Lehe alaosas on "Schemas" sektsioon
   - Seal näete kõigi DTO-de ja mudelite struktuuri
   - Kasulik API integreerimise jaoks

**Swagger annotatsiooni näited koodis:**
```csharp
[HttpPost]
[ProducesResponseType(typeof(AuthResponse), 200)]
[ProducesResponseType(400)]
public async Task<IActionResult> Login(LoginDto loginDto)
```

See tagab detailse dokumentatsiooni ja selge API kasutamise.

## 4. KOKKUVÕTE

Käesoleva projekti raames õnnestus edukalt arendada tänapäevane ja funktsionaalne restoranirakendus, mis ühendab endas kaasaegsed tehnoloogiad ja parimad arenduspraktikad.

**Projekt hõlmab:**
- Täielikku CRUD funktsionaalsust restoranide, menüüde, tellimuste ja kasutajate haldamiseks
- Turvalist autentimist JWT Bearer tokenitega
- Intuiitivset kasutajaliidest React.js ja Tailwind CSS-iga
- Tõhusat andmebaasi struktuuri MySQL-iga
- Interaktiivset API dokumentatsiooni Swagger'iga
- Moodsat arenduskeskkonda Bun runtime'iga

**Saavutatud tulemused:**
- Kiire ja skaleeritav backend ASP.NET Core 10.0 baasil
- Responsiivne ja kasutajasõbralik frontend
- Turvaline kasutajate autentimine ja autoriseerimine
- Reaalajas tellimuste haldamine
- Automaatne API dokumentatsioon

**Tuleviku arendussuunad:**
- Reaalajas teavitused WebSocket'ide kaudu
- Mobiilirakenduse arendamine
- Maksesüsteemi integreerimine
- Analüütika ja aruandluse funktsionaalsus
- Multi-tenant arhitektuuri implementeerimine

Projekt demonstreerib tänapäevaseid veebirakenduse arenduse põhimõtteid ja pakub kindlat alust edasiarenduseks ning tootmiskeskkonda juurutamiseks.