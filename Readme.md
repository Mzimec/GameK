#Přehled

Tento projekt se pokouší vytvořit architekturu pro budoucí RPG tahovou hruv Enginu Unity.
Cílem bylo připravit dostatečně modulární systém, který by dovoloval skládat v samotném Unity editoru mnoho komplexních assetů bez použítí kódování.
Architekturu samotná se pokouší co nejvíce oddělit od samotných Unity komponent a prvků pro lepší Unit testing.

V rámci projektu se věnujeme:
- Herním charkterům

- Herním akcím

- Základnímu náčrtu na AI herních agentů

- Systém dočasných efektů

- Předmětům, které můžou chraktery využívat

- Dialogovým oknům

- Tahovému módu

- Gridu
 
##Charakter
 
CharacterCore je centrální modelová třída, která reprezentuje herní postavu v systému. 
Neslouží jako MonoBehaviour, ale jako čistý C# objekt, aby byla logika hry nezávislá na Unity enginu a snadno testovatelná. 
Tento přístup zároveň umožňuje jednodušší serializaci (save/load) a lepší oddělení datové a prezentační vrstvy.
 
###Moduly
 
CharacterCore slučuje základní moduly, kterédefinují každou herní postavu.
Prozatím se skaldá z:
- Staty(IStatsWrapper) -- Základní přístup, ke všem statům nacházející se na charakteru.
- Atributy(CharacterAttributes) -- Stavební staty pro postavu.
- Vitalita(IVitals) -- Jedná se o komponentu, která pracuje se zdravím.
- Akce(IActions) -- Správce jednotlivých herních akcí, které charakter může podniknout.
- Poškození(IDamageReceiver) -- Správce příjímaného poškození.
- AI(ICharacterAI) -- Správce rozhodování nad akcemi u charakterů neovládaných hráčem.
- EventBus(BaseEventBus) -- mechanismus pro publikování a odběr událostí na úrovni postavy
(např. když postava utrpěla zranění, když začíná tah atd.).

Postupně se musí dodat:
- Efekty(IEffects) -- Správce aktivních efektů.
- Batoh(IInventory) -- Správce předmětů ve vlastnictví charakteru.
- Doalog(IDialog) -- Správa spouštění Dialogových oken.
 
###Použité navrhové vzory
 
- Composition over Inheritence -- místo velké hierarchie dědičnosti je CharacterCore složen z komponent (staty, vitály, akce). 
To usnadňuje rozšiřitelnost a znovupoužitelnost.
 
- Interface Segregation -- každý aspekt postavy má vlastní malé rozhraní (IHasStats, IHasVitals, IHasActions, …). 
To zajišťuje, že ostatní systémy (např. boj, UI, AI) mohou pracovat jen s tím, co opravdu potřebují. 
 
- EventBus (Observer pattern) -- pomocí EventBus je umožněna volná vazba mezi systémy. 
Například AI se může přihlásit k odběru „OnDamageTaken“ a reagovat, aniž by musela znát interní detaily postavy. 
Využívám package R3, který ulehčuje práci s Observrem ačkoli, pro jednodušší globalní eventy mám připravenou svůj globální event bus.

- Dependency Injection (částečně) -- Jednotlivé Staty postavy (atributy, redukce poškození...) jsou při jejich inicializaci
předávány do komponenty IStatsWrapper, která nám pak umožnuje spouštět operace přes všechny staty na charakteru.
 
###Designové cíle
 
- Modularita – každý aspekt (staty, vitály, akce) je samostatný modul, který se dá vyvíjet a testovat izolovaně.
 
- Rozšiřitelnost – do budoucna je možné přidat inventář, efekty, dialog tree, aniž by bylo nutné zasahovat do jádra.
 
- Decoupling od Unity – logika postavy běží nezávisle na enginu, což umožňuje testování čistě v C# prostředí a snadný přenos do jiných herních režimů.
 
- Event-driven architektura – převážně platí mezi dočasnými efekty reagujícími na spouštěcí akce.
 
##Akční systém
 
Tento modul reprezentuje akční systém hry. 
Cílem je poskytnout flexibilní a rozšiřitelný způsob, jak popsat, vykonávat a predikovat herní akce – ať už jde o útoky, kouzla, obranné reakce nebo složené sekvence akcí.
 
###Klíčové komponenty

- IGameAction je základní rozhraní pro všechny herní akce. 
Definuje metodu Execute pro skutečné provedení akce v herním kontextu (ActionContext).
Definuje metodu GetApproximatedResult pro získání odhadu výsledku (např. očekávané poškození, pravděpodobnost zásahu).
Tento dvojí pohled (skutečné provedení vs. odhad) umožňuje AI nebo UI předem vyhodnocovat akce.

- ActionNode reprezentuje uzel v akčním stromě. 
Podporuje vlastní logiku (SelfExecute, ApproximateSelfExecute), kterou lze přetížit (např. udělení dmg, heal, buff).
Obsahuje děti (_children) rozdělené podle výsledku hodu (úspěch, neúspěch, kritický úspěch/selhání).
Integruje targeting (ITargetingSelector) – určuje, na koho se akce aplikuje.
Integruje roll resolution (IRollResolver) – umožňuje simulovat RPG logiku (hod na útok, saving throw).
 
- ActionNodeBuilder implementuje Implementuje Builder pattern, aby byla konstrukce ActionNode čitelná a plynulá.
 
- GameAction reprezentuje herní akci, kterou si může zvolit charakter a provést ve svém tahu.
Obsahuje metadata a funguje jako root pro strom kompozitních akcí.
 
###Použité návrhové vzory
 
- Composite -- Akce funguje jako strom, kterým procházíme spouštíme chtěné akce.
 
- Builder -- ActionNodeBuilder usnadňuje tvorbu složitých uzlů.
 
- Strategy -- IRollResolver a ITargetingSelector jsou strategie – 
zaměnitelné komponenty, které určují chování akce bez zásahu do logiky ActionNode

- Command – každá akce (IGameAction) je v podstatě příkaz, který lze vykonat nebo analyzovat.
 
- Chain of Responsibility (částečně) – díky vnořeným uzlům a rolím lze sledovat, jak akce postupně předává kontrolu dětem podle výsledku hodu.
 
###Designové cíle
 
- Flexibilita – akce lze deklarativně skládat a kombinovat (damage → status effect → další větvení podle úspěchu).
 
- Predikovatelnost – systém umožňuje akce nejen provádět, ale i předvídat jejich důsledky (AI, tooltips, simulace).
 
- Rozšiřitelnost – přidat novou akci znamená jen zdědit z ActionNode a přepsat logiku.
 
- Low coupling – díky rozhraním (ITargetingSelector, IRollResolver, IDamageReceiver) lze akce snadno znovupoužít v různých kontextech (boj, skriptované eventy).
 
##Efekt systém
 
Tento modul reprezentuje systém efektů, který zajišťuje aplikaci dočasných nebo trvalých změn na postavy (CharacterCore). 
Efekty mohou upravovat statistiky, spouštět akce při určitých událostech, nebo provést jednorázovou či reverzibilní změnu pomocí jiných akcí. 
V jednoduchosti jsou efekty něčím, co dočasně ovlivňuje charakter.
 
###Klíčové komponenty
 
IEffect je základní rozhrní pro herní efekty, zaručující správné chování při přidání a odebrání efektu.
 
StatModifierEffect<T> je konkrétní efekt, který zastupuje častý typ efektu měnící stat.
Jedná se svým způsobem o Decorator pattern, obaluje základní logiku statu doatečnou logikou.
 
TriggerdActionEffect<T> Zastupuje Efekty, které se zvládají přihlašovat nějakou akci k eventu event busu.
Opět využíváme Observer pattern při praci s EventBusem.

ActionEffect zastupuje komplexní efekty, které jako při přidání i odebrání spouštějí specifickou IGameAction.

###Designové cíle
 
Modularita -- každý efekt je samostatná jednotka, která řeší jen jednu věc (staty, akce, reakce na eventy).

Reverzibilita -- všechny efekty implementují logiku Remove, což umožňuje bezpečné zrušení (důležité pro buffy/debuffy s časovým limitem).

Reusability -- efekty lze kombinovat s libovolným IGameAction, IStatModifier nebo IActionEvent, takže systém není omezen na konkrétní logiku.

Low coupling -- efekty jsou závislé jen na abstrakcích (CharacterCore, IStatsWrapper, IEventBus, IGameAction), nikoliv na konkrétních implementacích.
 
##Systém statistik
 
Systém statistik definuje, jak jsou v RPG hře reprezentovány, měněny a spravovány všechny atributy entit (postav, nepřátel i objektů).
Poskytuje jednotnou vrstvu pro:

Jednoduché statistiky -- pouze základní třída, o které víme, že má nějakou hodnotu typu T.

Modifikovatelné statistiky (mohou být buffnuté nebo debuffnuté) -- Drží si stav modifikátorů, které jim upravují výslednou hodnotu.
Mají danou logiku, ve které vyhodnocují modifikátory.

Zdroje (např. Životy nebo Mana – mají maximální a aktuální hodnotu) -- maximalní hodnota je svým modifikovatelný stat, který můžeme upravovat modifiakátory.
Naopak s aktuální hodnotou pracujeme přímo -- jednoduché pro práci s často se měnící hodnotou.

Regenerovatelné statistiky (např. regenerace many za kolo) -- Přidává vlastnost obnovování.

Porovnatelné/statistiky s omezením (ohraničené mezi minimem a maximem) -- Nejběžnějsí numerické staty, 
dědí z modifikovatelných statů navíc mají horní a spodní hranici a zaručuje porovnávací schopnost.

Modifikátory (flat bonusy, procenta, clamping …) -- Mají mnoho typů, které se liší v dekorační metodě ModifyValue().
Každý typ má svou logiku změny.

Tento systém tvoří základ pro levelování postavy, výpočet výsledků boje a aplikaci efektů.
 
###Designové cíle
 
Vytvořit silný generický tzpový základ pro práci s podstatnými hodnotami v charakteru.
 
###Wrapper Statistik

IStatWrapper read only kontejner mapujicí druh statu k statu samotnému. 
Drží referenci pro přístup k libovolnému statu na charakteru. 
Ideální pro přístup k statům bez znalosti z jakého modulu charakteru pochází.
Například idelní pro přístup efektu ke statu.
 
##AI
 
Tento modul definuje rozhraní a základní implementace pro umělou inteligenci (AI) postav v tahovém RPG.
 
AI má na starosti vyhodnocení dostupných akcí, výběr nejvýhodnější akce a rozhodnutí, kdy ukončit tah.
 
### Klíčové části
 
ICharacterAI je rozhraní definující základní chování AI postavy:
 
- TakeTurn -- spouští logiku tahu (může obsahovat více akcí).

- TakeAction -- provede jednu akci a může rozhodnout o konci tahu.

- Použitý Strategy Pattern -- umožňuje vyměnit implementaci AI (pro jiný typ evaluace atd.).
 
GameState je zástupná třída pro stav hry (např. rozmístění jednotek, stav bojiště, aktivní efekty).
Bude rozšiřována o detailní informace nutné pro rozhodování AI.
 
IActionComparer je rozhraní pro hodnocení akcí – dostane aproximaci výsledku akce a postavu, která ji provádí.
Vrací skóre (float) určující atraktivitu akce.
Používá Scoring/Evaluation Pattern, často využívaný v šachových enginech a strategických hrách.

IGridOperations je ozhraní pro práci s mapou (gridem).
Umožňuje získat potenciální políčka, kam může akce působit.
Oddělení grid logiky = Separation of Concerns. AI neřeší detaily mapy, jen získává kandidátní pozice.
 
IActionOperations je rozhraní pro práci s cíli akcí.
Vrací seznam možných cílů akce (např. nepřátelé, spojenci, objekty).
Další příklad inverze závislostí – AI neřeší, kdo může být cílem, jen využívá tuto službu.
 
BaseCharacterAI je abstraktní základ pro konkrétní implementace AI.
Definuje algoritmus:

- Vyhodnotit akce a cíle → EvaluateAction

- Vybrat nejlepší akci → SelectBestAction

- Najít potenciální cíle → GetPotentialTargets

###Použité návrhové vzory

- Strategy -- různé AI mohou implementovat ICharacterAI.

- Template Method -- BaseCharacterAI definuje základní strukturu výpočtu, detaily přenechává podtřídám.

- Dependency Injection / Inversion of Control -- AI nevlastní logiku skórování, gridu ani cílů, tyto závislosti se předávají jako rozhraní (IActionComparer, IGridOperations, IActionOperations).

- Scoring System -- inspirováno herními AI a šachovými enginy, kde se akce hodnotí podle výsledného skóre.

- Separation of Concerns -- AI nerozumí mapě ani cílům, jen využívá specializované služby.
 
#Co je třeba dodělat
 
##Inventory
 
Představa je taková, že každý předmět bude mít Metody OnEquip a OnUnequip, které charakteru dají popřípadě uberou nějaké efekty.
V případě UsableItem mu přidají i nové akce. Ale chování je vlastně velmi podobné jako chování efektu, aneb dočasné změna stavu charakteru.
 
##Modul charakteru Actions
 
Ačkoli samotné akce jsou již celkem naplánovány, musí se vztvořit kvalitní rozhraní,
které bude mít uložené akce. A shopnost vracet, které akce jsou v kole ještě pouzitelne.
 
##Modul Dialogů
 
Plán je vytvořit stromovou strukturu dialogů, které budou ukládany v .json souborech.
 
##Evaluační funkce pro AI

Možností je více. A bylo by zajímavé vyzkoušet různé složitější algoritmy MonteCarlo, a nebo aspoň funkce zpracovávat synergie spojenců,
určovat společný cíl atd. Nyní je to připraveno pro jednoduchou hodnotící funkci, která odhadne výsledek funkce a ohodnotí ji na paramatrech.
Není tedy kontextová.
 
##Battle Manager
 
Správce tahového souboje. Představa není nic složitého. Šlo by o frontu charakterů, určenou podle jejich iniciativy.
S Metodami pro ukončení a inicializaci souboje.
 
##Grid a Grid Operace
 
Toto bude ještě hodně práce. Půjde o zástupce, mapy, který si udržuje stav o jednotlivých dlaždicích.
Nevím jestli to dává 100% smysl, ale v plánu mám vytvořit si časté TileData jako ScriptableObject. 
Pak bude stačit aby každý Tile měl jen referenci na takovou to statickou třídu. 
A v samotném GridManageru bych si kdyžtak držel jen odchylky od stavajících GridData.
 
Grid Operace, tím myslím vytvořit další užitečné operace pro rychlou práci s Gridem.
Hledání cesty, Ideální cesty, Získávání růyných tvarů AoE akcí. Určení zda je volná trajektorie pro střelbu...
 
##Drobné třídy, které rozvadějí tuto strukturu.
 
Například definice základních akcí. V tuto chvíli máme jen DealDamage.
 
##Logging
 
Mnoho struktur, které za běhu vytváříme už počítají s Loggingem, ale zatím není samotný systém v provozu.
 
#Samotná hra
 
Stávající "Engine" je velmi podobný v základu DnD. Rozhodl jsem se to v rakovém to ramci 
rozpracovat, jelikož jsem se mohl věnovat vzmýšlení Architektury a nikoliv systému.
Výsledná hra však mít mnoho pravidel jiných, hlvně co se týče action economy a toho, které akce hráč může používat.
Nicméně DnD systém byl vhodným kandidátem pro načrtnutí základního rozhraní, které později upravím.
 
 
 
 
 