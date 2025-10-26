# Gameplay Design Document

## 1. Popisek

Hra Kryplague bude izometrickým tahovým RPG inspirovaný tituly jako Bladur's Gate I a II, Pathfinder: Kingmaker, Pathfinder: Wrath of Righteous.
Hra bude postavena na vlstním herním systému, který přidává větší hlobku taktiky do tahových soubojů. 
Herní svět bude připomínat potemnělé ranně rensanční Evropu, kterou prolézá morová pohroma. Každý se ptá, jak mohl bůh něco takového dopustit.
Dochází k rozpadu společnosti, každý se vypořadává s hrozícím nebezpečím svým způsobem. 

---

## 2. Jádro herního zážitku

Hra klade důraz na:
- **Taktické myšlení:** Souboje vyžadují analýzu situace a správné využití akcí.  
- **Strategii zdrojů:** Hráč hospodaří s omezenými akčními body, kouzly a buffy.  
- **Variabilitu výsledků:** Každá akce má šanci selhat či uspět podle hodů kostkou.  
- **Dlouhodobý rozvoj:** Postavy se zlepšují, získávají nové schopnosti a vybavení.  
- **Příběhové volby:** Morální rozhodnutí a interakce mají vliv na výsledek kampaně.  

---

## 3. Gameplay Loop

### 3.1 Primární smyčka
1. **Průzkum světa**  
   Hráč se pohybuje po mapě, komunikuje s NPC, sbírá informace a vybavení, plní úkoly.  
2. **Setkání**  
   Příběhová či náhodná událost vede k boji, hádce, pasti nebo interakci.  
3. **Rozhodnutí / Souboj**  
   - Pokud je situace konfrontační, přechází do **tahového souboje**.  
   - V jiných případech může proběhnout **hod na dovednost** (např. přesvědčování).  
4. **Odměna a rozvoj**  
   Úspěšné dokončení akce přináší zkušenosti, loot a příběhový posun.  
5. **Správa družiny**  
   Hráč leveluje postavy, vybavuje je nalezenými předměty a tvoří efektivní tým pro překonání překážek.  

Tento cyklus se opakuje — střídá se **napětí souboje** a **klid správy**.

---

## 4. Tahový souboj

### 4.1 Princip
Každý boj probíhá v tazích. V jednom kole se vystřídají všechny frakce (hráč, nepřátelské pořpípadě spojenecké skupiny).  
Během kola táhnou všechny charaktery: Jednotlivé frakce se střídají v tazích, vyberou si charkter, kterým táhnou a předají iniciativu dál.
Pokud není charkterů v jednotlivých bojových skupinách stejně, těm, které dojdou charktery končí kolo.
Každá postava má určité **Akční Body (AP)**, které určuje, kolik akcí může provést.

### 4.2 Akce
Každý charakter má svůj strom ackí, které si během levlování odemyká.
Každá taková akce je cestou z jednoho bojového postoje do druhého.
Jednotlivé bojové postoje dávají postavám kombinaci pasivních buffů a debuffů, dokud v tomto postoji stojí.
Dejme tomu že tedy akcí *Uppercut* se můžeme dostat z bojového postoje *Alber* do *Vom Tag*.
- **Pohyb** – stojí určité množství AP podle vzdálenosti. Nemění bojový postoj.    
- **Interakce** – otevření truhly, aktivace zařízení, pomoc spojenci. Nemění bojový postoj. 

### 4.3 Rozhodovací vrstvy hráče
- **Taktická úroveň:** výběr cíle, pozice, pořadí akcí v rámci kola.  
- **Startegická úrove:** uvažování do budoucích kol (v jakém postoji chci skončit - ať to ovlivňuje buffy či jaké akce chci provádět v příštím kole).
- **Riziková úroveň:** posouzení šance na úspěch (pravděpodobnost hodu).  
- **Roleplayová úroveň:** posouzení toho, jak chci aby se můj charkter v souboji zachoval, nebo jaký bude jeho výsledek.

### 4.4 Typy hodů
Některé akce vyžadují nějaký hod, aby určily svou úspěšnost. Prozatím se držím myšlenky hodů DnD.
- **Útočný hod (Attack Roll):** porovnává d20 + bonus s cílovým AC.  
- **Záchranný hod (Saving Throw):** cíl se brání proti efektu (DC kouzla).  
- **Kontrolní hod (Skill Check):** používá se mimo boj (např. Lockpicking, Persuasion).  

### 4.5 Výsledky hodu
| Typ výsledku | Popis | Efekt |
|---------------|--------|--------|
| **Critical Success** | d20 = 20 | Dvojitý efekt nebo extra bonus |
| **Success** | Hod >= cílové hodnotě | Akce proběhne úspěšně |
| **Failure** | Hod < cílové hodnotě | Akce selže |
| **Critical Failure** | d20 = 1 | Negativní následek nebo postih |

---

## 5. Postavy a rozvoj

### 5.1 Atributy
Základní statistiky charakteru, od kterých se odvíjí mnoho dalších vlastností.
- **Power (Moc):** určuje poškození většiny schopností.  
- **Accuracy (Přesnost):** určuje přesnost většiny schopností.  
- **Dexterity (Obratnost):** určuje šanci na vzhnutí se schopnostem  
- **Sturdiness (Zdatnost):** určuje redukci příchozího poškození, medicinské a zalésacké dovednosti.  
- **Willpower (Vůle):** určuje šanci uspět proti negativním efektům shopností.  
- **Charisma (Charisma):** určuje šanci úspěchu v konverzacích a slevu při nákupech.
- **Arcana (Metafyzika):**  určuje schopnost postavy ovládat magické nauky.
- **Faith (Víra):** určuje porozumnění postavy Božím přičiněním.

### 5.2 Vedlejší statistiky
Každá postava bude mít své vedlejší statistiky vycháyející z hlavních attributů.
Například critical chance, crit multiplier, resistances...

### 5.3 Schopnosti (Abilities)
Každá schopnost je definována jako **akce**, kterou může postava použít.  
Mají parametry:
- Typ (útok, kouzlo, buff, utility).  
- Cíl (single target, area, self). 
- Typ cíle (spojenec, nepřítel, charakter, tile...). 
- Cena (v AP, nebo speciální zdroj).  
- DC (obtížnost pro záchranný hod).  
- Vedlejší efekty (poškození, omráčení, hoření…).  

### 5.4 Rozvoj
Postava se bude dostavat zkušenosti při plnění úkolů, nikoli při zabíjení nepřatelů.
Při level upu postava získáva životy, může rozdistribuovat daný počet bodů do svých attributů, 
získat pasivní perk nebo si odemknout novou sadu úderů.  

---

## 6. Efekty, buffy a debuffy

### 6.1 Princip
Buffy a debuffy dočasně mění hodnoty statistik, často skrze modifikátory.  
Mohou mít trvání (v kolech) nebo podmínky (dokud trvá kouzlo).  

### 6.2 Typy efektů
- **Posilující:** Bless, Haste, Heroism  
- **Oslabující:** Poison, Slow, Curse  
- **Kontrolní:** Stun, Fear, Charm  

### 6.3 Stacking pravidla
- Stejné efekty ze stejného zdroje se **ne-stackují**.  
- Různé zdroje mohou vytvářet **multiplikativní efekty**.  

---

## 7. Pacing a filozofie boje

### 7.1 Délka boje
- Krátký střet: 2–4 kola.  
- Velký střet: 5–8 kol.  
Cílem je, aby hráč měl **čas plánovat, ale necítil únavu** z mikromanagementu.

### 7.2 Dynamika
- Boje začínají výhodou iniciativy.  
- Každý tah přináší viditelný dopad (damage, kontrola, buff).  
- Klíčem je **přehlednost informací** — hráč by měl chápat, proč uspěl/neuspěl.  

### 7.3 Umělá inteligence
- AI analyzuje pravděpodobnost úspěchu (`ActionApproximation`).  
- Vyhodnocuje riziko a odměnu podle stavu bitvy.  
- Upřednostňuje cíle s nižším HP nebo strategickou pozicí.  

---

## 8. Odměny a motivace

- **XP** – zisk zkušeností za poražené nepřátele.  
- **Loot** – zbraně, brnění, kouzla.  
- **Příběhový postup** – nové oblasti, vztahy, možnosti dialogů.  
- **Morální reputace** – ovlivňuje reakce NPC a konce příběhu.  

---

## 9. Předměty

- **Příběhové předměty:** potřebné pro slnění úkolu.
- **Zbraně:** ovliňují útočné akce svými statistikami při užívání.
- **Zbroj (Equipable):** dává charkteru efekty, dokud je má na sobě. 
- **Usable:** míněné pro jednorázové užití, při kterém spustí nějakou akci.
