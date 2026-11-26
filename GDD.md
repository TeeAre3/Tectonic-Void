# GAME DESIGN DOCUMENT (GDD)

## Version History
| Wersja | Data | Autor | Opis zmian |
| :--- | :--- | :--- | :--- |
| `0.1` | 2026-11-26 | Zespół | Wstępna konwersja GDD z PDF do Markdown i konfiguracja repozytorium. |

---

## Informacje Ogólne
* **Nazwa Projektu:** Tectonic Void
* **Gatunek:** Top-Down Action Rogue-lite / Spatial Puzzle
* **Docelowe Platformy:** PC (Steam), Konsole
* **Styl Wizualny:** Stylizowany 2D / 2.5D Pixel Art lub Low-Poly 3D, z zimną paletą barw

---

## 1. WIZJA PROJEKTU I CORE CONCEPT

### 1.1. High Concept
**Tectonic Void** to dynamiczny action rogue-lite z widokiem z góry, w którym gracz przemierza dryfujące w próżni monolity starożytnej cywilizacji. Przestrzenie są polem bitwy i zagadką – gracz kontrolować może same płyty, po których się porusza, by unikać ciosów, łączyć przejścia i odcinać wrogów.

### 1.2. Filary Rozgrywki
1. **Dynamiczna Tektonika (Spatial Control):** Manipulowanie terenem w czasie rzeczywistym jest równie ważne jak machanie bronią.
2. **Kosmiczny Horror i Odchylenie (Unstable Void):** Nadużywanie przestrzennej manipulacji budzi uśpioną tkankę próżni (*Tectonic Decay / Corruption*).
3. **Płynna Walka w Zwarciu (Flow Combat):** Krótkie czasy reakcji, uniki (*dash*), kontry i płynne przechodzenie z platformy na platformę.

---

## 2. ŚWIAT I NARRACJA

### 2.1. Zarys Fabularny
Ludzkość odkryła Nie-Miejsce (*The Void*) – obcą strukturę dryfującą poza granicami znanego wszechświata. W jej wnętrzu znajdują się megalityczne płyty tektoniczne zbudowane z nieznanych stopów. Gracz wciela się w Nawigatora Próżni – wysłannika wyposażonego w aparat zagięcia przestrzennego (*Void Grip*), który próbuje zejść w głąb monolitu, by poznać prawdę o tzw. Architektach Próżni.

### 2.2. Klimat i Estetyka
* **Architektura:** Geometryczne, monolityczne platformy, starożytne runy, rozpadające się pomniki połączone z biomechanicznymi tkankami.
* **Audio:** Niepokojący dron kosmiczny, stłumione dźwięki próżni, organiczne mlaśnięcia wyrastających macek, ostre dźwięki uderzeń bronią o stal/kamień.

---

## 3. MECHANIKA ROZGRYWKI

### 3.1. Poruszanie i Manipulacja Przestrzenią
* **Poruszanie Postacią:** Gracz porusza się swobodnie po płaszczyźnie aktualnej platformy.
* **Przesuwanie Płyt (Tektonika):** Po aktywacji sterowania platformą, aktualnie zajmowana płytka przesuwa się w osiach $X/Y$ w kierunku wskazywanym przez gracza.
* **Fizyka Kolizji Płyt:**
  * Połączenie krawędzi dwóch platform pozwala graczowi (oraz wrogom) na natychmiastowe przejście.
  * Zderzenie dwóch płyt z dużą prędkością może zmiażdżyć stojących na krawędziach mniejszych wrogów (*Mechanika Tectonic Crush*).

### 3.2. Twist: Mechanika Skazy Próżni (Void Corruption / Strain)
Zamiast standardowego współczynnika poczytalności, gracz posiada wskaźnik **Naprężenia Tektonicznego** (*Tectonic Strain*).
* **Aktywacja:** Każdy ruch platformą wypełnia pasek Naprężenia.
* **Efekt Przeładowania (Corruption Burst):**
  * Gdy pasek osiągnie 100%, krawędzie platformy „ożywają” – wyrastają z nich zakrwawione macki/tkanka Lovecraftowska.
  * **Konsekwencje:** Platforma zostaje zablokowana (nie można jej przesuwać), a macki zadają obrażenia graczowi, jeśli zbliży się do krawędzi.
* **Mechanika Odblokowania (Stabilization):**
  * Gracz musi użyć Zastrzyku Stabilizującego (*Void Sedative*) lub rzucić Rytuał Uspokojenia (wymaga wciśnięcia odpowiedniej sekwencji klawiszy).
  * Wykonanie zabiegu wymaga trwającej ~1.5–2 sekundy animacji (gracz jest wtedy wrażliwy na ataki), co wymusza planowanie przestrzenne.

### 3.3. System Walki
* **Broń Główna (Melee Focus)** – gracz może posiadać 2 sztuki jednocześnie:
  * *Miecze jednoręczne:* szybkie serie, dash-strike.
  * *Lanca Próżniowa:* duży zasięg, odpychanie wrogów z krawędzi platform.
  * *Ostrza Tektoniczne:* wolne uderzenia obszarowe, powodujące drgania płyt.
* **Broń Dodatkowa / Palna / Zaklęcia (Support/Ranged):**
  * Broń palna pełni rolę taktyczną (np. Pistolet Grawitacyjny przyciągający wroga do krawędzi, Broń Harpunowa przeskakująca na sąsiednie płyty). Posiada ograniczoną amunicję regenerowaną przez atakowanie bronią białą.
* **Unik / Dash:** Pozwala przeskakiwać niewielkie przerwy między platformami oraz unikać ciosów.

---

## 4. STEROWANIE (CONTROLS SCHEME)

### Klawiatura i Mysz
| Klawisz | Akcja | Opis / Uwagi |
| :--- | :--- | :--- |
| `W`, `S`, `A`, `D` | Poruszanie postacią | Ruch po płaszczyźnie platformy |
| `Strzałki` | Przesuwanie platformy | Przesuwa platformę, na której aktualnie stoi gracz |
| `Q` | Lekki atak bronią białą | Podstawowy combo-atak |
| `E` *(lub Przytrzymanie Q)* | Ciężki atak bronią białą / specjalny | Wolniejszy, ale potężniejszy cios |
| `X` | Zmiana broni | Przełączanie broni lub szybki skrót do stabilizacji |
| `F` | Interakcja | Otwieranie skrzyń, aktywacja konsoli, podnoszenie przedmiotów |
| `Spacja` / `Shift` | Unik / Dash | Zapewnia ramki nietykalności (*i-frames*) |
| `R` | Zastrzyk Stabilizujący / Zaklęcie | Użycie sedatywu w celu usunięcia macek z platformy |

### Układ na padzie (Controller Layout – *do ewentualnej poprawy*)
* **Lewy Analog:** Ruch postaci
* **Prawy Analog (lub D-Pad):** Ruch platformą
* **X:** Atak
* **A:** Dash
* **RB:** Broń palna / spell / Interakcja *(do rozdzielenia)*
* **Y:** Stabilizacja

---

## 5. PĘTLA ROZGRYWKI

```text
[Start Runu / Baza] ──► [Wejście do Monolitu (Poziom)]
       ▲                            │
       │                            ▼
[Śmierć / Rozwój] ◄── [Walka + Zagadki + Zarządzanie Skazą] ──► [Walka z Bossem / Awans]
```

1. **Eksploracja i Zagadki:** Gracz przesuwa płyty, aby przemieścić się na kolejne sekcje poziomu, otwierać zamknięte bramy i omijać przepaście.
2. **Starcia:** Łączenie platform w celu zwabienia wrogów na jedną płytę lub izolowanie groźnych przeciwników na osobnych platformach i spychanie ich w próżnię. Przeciwnicy wykrywają gracza, gdy ten złączy się z ich platformą lub jest w ich zasięgu ataku (dla przeciwników strzelających).
3. **Zarządzanie Ryzykiem:** Decyzja – czy przesuwać platformę częściej dla przewagi taktycznej, ryzykując blokadę mackami, czy walczyć w trudniejszej pozycji.

---

## 6. PRZECIWNICY I BOSOWIE

### 6.1. Standardowe Jednostki
* **Husk of the Void:** Podstawowy crawler, łudząco przypominający zniekształconego człowieka, atakujący masowo.
* **Plate Walker:** Duże, opancerzone monstrum. Wykonuje ataki obszarowe i silniejsze, przeskakujące na sąsiednie platformy.
* **Tendril Hatchling:** Dryfujące stworzenie Lovecraftowskie, które atakuje gracza na odległość, porusza się powoli.

### 6.2. Bosowie (Przykłady)
* **The Shattered Architect:** Gigantyczna istota zawieszona w tle, która sama potrafi przesuwać platformami gracza lub je niszczyć, zmuszając do ciągłej ucieczki i precyzyjnego manewrowania.

---

## 7. PROGRESSION I METAGAME

### W trakcie runu (In-Run Progression)
* **Moduły Tektoniczne:** Ulepszenia wprowadzające modyfikatory do broni oraz ruchów platform (np. płytka po zderzeniu wywołuje falę uderzeniową), podnoszone z elitarnych przeciwników (silniejsze wersje zwykłych: więcej HP oraz silniejsze ataki, podświetleni na wybrany kolor).
* **Zastrzyki / Serum:** Modyfikacje stabilizatora (np. zastrzyk przywraca 15% HP, ale wydłuża animację).

### Między runami (Meta-Progression)
* Zbieranie **Próżniowych Rdzeni (Void Cores)** z bosów.
* Odblokowywanie nowych typów broni startowych, zwiększanie pojemności zastrzyków oraz odporności na Naprężenie.