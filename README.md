# `BlockBusterXaml` project  
This is just xaml based debugger for playtesting

![Screen](readme/screen.jpg?raw=true "Title")


# `BlockBuster` project  
Actual tetris designed as class library that can be used anywhere. I plan to test it with Unity as well as xaml(XAML tested)  


## Features
- [Super Rotation System](https://tetris.fandom.com/wiki/SRS)
- 7-Bag spawn system
- Naive gravity
- Hard drops
- Wall kicks

## Field
It have 2 invisible top rows to spawn tetrominoes, it is enough for SRS system

## TODO
- [x] Implement SRS rotation
- [x] Implement gravity and speed levels
- [x] Ghost tetromino
- [x] Implement new cell type for playfield to be rendered, to it can have move properties
- [x] Game status event
- [x] Sound events
- [x] Implement wallkicks
- [x] Constrained(time/repetition) infinity(With toggle) https://tetris.fandom.com/wiki/Infinity
- [x] Fix windows spawn location
- [x] Make xaml next queue preview
- [x] Xaml hold preview
- [x] Xaml menu: start\restart\pause on esc
- [x] SpawnTetromino(): Implement 7 pack spawn system
- [x] Check performance
- [x] Create readme
- [ ] Implement basic scoring
- [ ] Implement debug playfield states
- [ ] Implement scoring tests
- [ ] Check that soft drop does not lock mino
- [ ] Auto repeat rate for rotation\horizontal and vertical movement
- [ ] Save score to file with regard to game mode
- [ ] Game modes: Quick play\Endless\Marathon\Sprint\Ultra 
- [ ] Settings class with defaults in txt file

## TODO Low priority
- [x] Figure out snake sequences and what to do with them in spawn queue (more than 2 snakes consequently not permited)
- [x] Tests (maybe?)
- [ ] Fix unit tests
- [ ] Try making infinity like in tetris effect: your lock times got reset for every move you do, up to 16 tries
- [ ] Probably I do too many checks and code can be simplified
- [ ] Icon
- [ ] Read about fourht kick upgrading simple move to diffucult move, so it won't break the combo and b2b counter
- [ ] ObjectCopier: check obsolete code
- [ ] Fix spelling of 'tetromino' everywhere, or rebrand to Block
- [ ] Playtest variable levels up
- [ ] Delayed Auto Shift: the amount of time before an input is repeated/ARR is engaged.(Auto Repeat Rate)
- [ ] If you’ve noticed, there is a short time gap between the Tetrimino placed and the next spawned. This interval is called the entry delay, or usually called ARE (a term in Japanese indicating “that” gap). This is a gap where you can charge your movement, so when the Tetrimino comes out, it will automatically shift in that direction. 
- [ ] 20G does not have a ghost Tetrimino function for you. When you see the Tetrimino on the ground, it’ll be too late. But if your movement gets fast enough, you’ll get more time to look at the NEXTs. Remember,
- [ ] Initial hold system, Initial rotation system (?)

## Bugs
- [x] Check infinity, looks like after hard drop it waits for some time to spawn next mino and make sure that kick triggers infinity
- [x] Clicking on 'New game' after another game started causes +50mb ram
- [x] Level switched to 2 only after 20th mino