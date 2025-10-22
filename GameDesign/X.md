# Game Design Document

## 1. Přehled
**Název projektu:** Kryplague  
**Žánr:** Izometrické RPG s tahovým bojem (inspirováno Pathfinder, DnD 5e).  
**Platforma:** PC (Unity).  
**Jádro hry:** Hráč vede skupinu postav světem, řeší souboje, questy a rozvíjí postavy pomocí statistik a schopností.  

Jedná se o hru, která by byla zasazena do fiktivního světa v ranně rensančním období sužovaného Morovou ránou.
Cílem hry bylo vytvořit systém, ve kterém si charaktery, které hrají za válečníky nepřipadají jen jako damage dealeři,
kteří ve svém tahu jen klikají na tlačítko útok, ale musí přemýšlet jaký úder použijí.
Abych tohoto docílil inspiroval jsem se šermířským uměním. Každý úder, nebo i každá akce bude cestou z jednoho bojového postoje do druhého 
v grafu bojových postojů. Každý bojový postoj bude dávat charakteru něajké výhody nebo nevýhody. Hráč tedy bude muset přemýšlet nad tím, 
jaký postoj mu vyhovuje, zda má dobré efekty a zda z něj vychazí nějaké údery, které chce použít.

Hlavním rozdílem se tedy ve hře oproti jiným systémům stává souboj. Průzkum map a dialog bude probíhat podle očekávání.


---

## 2. Herní smyčka
1. **Průzkum** – pohyb po mapě, dialogy, interakce s prostředím.  
2. **Setkání** – boj nebo skill check (hod na schopnost).  
3. **Souboj** – tahový, založený na akcích a hodech kostkou (d20).  
4. **Rozvoj postav** – levelování, výbava, schopnosti, buffy/debuffy.  

---

## 3. Herní mechaniky

### 3.1 Statistiky
Každý charakter je definovaný pomocí řady statistik, které určují počet životů, s jakou přesností se trefí a podobně.
- **Základní atributy**: Dané statisktiky určující vlastnosti charakteru -- prototypálně Síla, Obratnost, Odolnost, Inteligence, Moudrost, Charisma. 
Nicméně nejsou definitvní 
- **Odvozené statistiky**: HP, AC (Armor Class), Attack Bonus, Saving Throws.
Statisktiky, které jsou odvozené ze hlavních statů, ale je možné jedále modifikovat.  
- **Stat System**:
  - `IStatBase` – wrapper pro staty libovolného `TValue`.
  - `IStat` – nemodifikovatelná hodnota.  
  - `IModifiableStat` – podporuje modifikátory.  
  - **Typy modifikátorů**:
  Jedná se způsob jak dočasně měnit staty z použití různých strategií.
    - Flat (přičte hodnotu).  
    - Percentage (násobí hodnotu).  
    - Setter (přepíše výsledek).  
    - Clamp (omezuje min/max).  

### 3.2 Akce
Každý charakter může během souboje během svého tahu využívat řadu akcí. Které mají vliv na něho i jeho okolí.
Nápad hry oproti klasickým DnD systémům je ten, že každa akce je cesta mezi dvěma vrcholy grafu šermířských postojů.
To znamená, že postava si v souboji udržuje nějaký stav aktuálního bojového postoje. Z něho může provést jen ty akce, které z něj vychazí.
Jednotlivé postoje přidávají postavam pasivní výhody, či nevýhody pokud v nich stojí.

Co mě vedlo k této myšlence je vzdání holdu šermu samotnému.

- **IGameAction** – rozhraní pro akce (útok, kouzlo, schopnost).  
- **ActionNode** – uzel akčního stromu, který:
  - Může vyžadovat hod (`IRollResolver`).  
  - Podle výsledku provede child-akce.  
  - Používá `ITargetingSelector` pro výběr cílů.  

### 3.3 Hody
Ovlivňují úspěšnost jednotlivých akcí, které je vyžadují.
- **RollRecord** – reprezentuje výsledek hodu d20.  
- **Výsledky**:
  - Critical Failure (přirozená 1).  
  - Failure (hod < target).  
  - Success (hod ≥ target).  
  - Critical Success (přirozená 20).  
- **Roll Resolvers**:
  - `AttackRollData` – hod proti AC cíle.  
  - `SaveRollData` – hod na záchranný hod proti DC.  

### 3.4 Souboj
Jedná se o tahový souboj, ve kterém se jednotliví aktéři (charaktery) střídají ve svých kolech, během kterých mohou podniknout
některé akce. Myšlenkou hry je, že každý hráč má počet akčních bodů, které může ve svém kole využít a každá akce ho stojí daný počet akčních bodů.
Boj v kolech pokračuje dokud není splněna jeho koncová podmínka ve většině případů, výhra jedné strany.
- **Tahový systém** založený na frakcích (hráč vs. nepřátelé).  
- Každá postava má **akční body**.  
- V tahu může provést:
  - Pohyb.  
  - Použití schopnosti.  
  - Interakci s inventářem.  
  - Aktivaci akce ze schopností nebo předmětů.  

### 3.5 Buff/Debuff systém
Systém, který napomáhá udržovat informace o dočasných efektech, které postihují charakter.
- **CharacterEffects** drží aktivní buffy/debuffy.  
- Buffy mohou přidávat modifikátory na staty.  
- Každý efekt má:
  - Zdroj (`sourceId`).  
  - Trvání.  
  - Typ (požehnání, jed, oslabení…).  

### 3.6 Levelování
Systém musí odrážet požadavky bojového systému. Hraáč nemá danou třídu a neyískává její výhody jako v jiných systémech.
Namísto tomu se mu zvedají jeho statistiky a odemyká si nové bojové postoje a akce.
  
