using UniMini;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MQ
{
    public enum CardState
    {
        OutOfBoard,
        Hidden,
        SpiedByOpponent,
        SpiedByPlayer,
        SpiedByPlayers,
        Revealed,
        ReadiedForPlayer,
        ReadiedForOpponent,
    }

    class CardModel
    {
        public AnimSprite sprite;
    }

    class Card
    {
        public Card()
        {
            state = CardState.Hidden;
        }

        public CardModel model;
        public CardState state;
        public bool isActivated;

        public bool isVisible => state > CardState.SpiedByOpponent;
        public bool isOnBoard => state > CardState.OutOfBoard;

        public int x;
        public int y;
    }

    class Deck
    {
        public CardModel[] cards;
    }

    class Player
    {
        public string name;
        public int hp;
        public Deck deck;
    }

    static class Theme
    {
        public static Color gameBackgroundColor = new Color32(37,40,45, 255);
        public static Color cardBackgroundColor = new Color32(56,56,56,255);
        public static Color activatePlayerColor = new Color32(255, 255, 255, 128);
    }
}

public class MemoryQuest : SpritzGame
{
    public bool drawDebugRects;
    public bool debugMode;
    public bool debugShowAllCards;

    MQ.Card[] m_Cards;
    int m_CurrentCardX;
    int m_CurrentCardY;
    int m_RevealedCardIndex1;
    int m_RevealedCardIndex2;
    int m_RevealedTimer;

    const int gridSize = 6;
    const int nbCards = gridSize * gridSize;
    const int deckSize = nbCards / 4;
    const int spriteSize = 48;
    const int cardWidth = 72;
    const int cardHeight = 96;
    const int revealedDurationInFrames = 45;
    float revealedDurationInSecond;

    Effect m_DissolveEffect;

    MQ.Player m_Player;
    MQ.Player m_Opponent;
    MQ.Player m_CurrentPlayer;

    RectInt m_OpponentRect;
    RectInt m_BoardRect;
    RectInt m_PlayerRect;
    RectInt m_InspectorRect;

    SpriteFont m_Font;
    int baseLayerId;
    int effectLayerId;
    int fontLayerId;

    public int currentCardIndex => m_CurrentCardX + m_CurrentCardY * gridSize;

    public override void InitializeSpritz()
    {
        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;

        revealedDurationInSecond = (revealedDurationInFrames + 5) * Spritz.secondsPerFrame;

        InitPlayers();
        InitUI();
        InitBoard();
        InitFont();
        InitEffects();
    }

    public override void UpdateSpritz()
    {
        HandlePlayerInput();

        UpdateEffects();
        UpdateCards();
    }

    public override void DrawSpritz()
    {
        // TO CHECK: awkward clear to avoid the top layer overridding all the background

        Spritz.currentLayerId = fontLayerId;
        Spritz.Clear(Color.clear);

        Spritz.currentLayerId = effectLayerId;
        Spritz.Clear(Color.clear);

        Spritz.currentLayerId = baseLayerId;
        Spritz.Clear(MQ.Theme.gameBackgroundColor);

        if (drawDebugRects)
        {
            DrawDebugRect();
        }
        else
        {
            DrawPlayerInfo(m_OpponentRect, m_Opponent);
            DrawBoard();
            DrawInspector();
            DrawPlayerInfo(m_PlayerRect, m_Player);
            DrawEffects();
        }
    }

    private void UpdateCards()
    {
        for (var c = 0; c < m_Cards.Length; ++c)
        {
            if (m_Cards[c].isVisible)
            {
                m_Cards[c].model.sprite.Update();
            }
        }
    }

    private void UpdateEffects()
    {
        if (m_DissolveEffect.playing && m_DissolveEffect.ticker.hasValue && m_RevealedCardIndex1 != -1 && m_RevealedCardIndex2 != -1)
        {
            m_DissolveEffect.Update();
        }
    }

    private void HandlePlayerInput()
    {
        // Update objects behavior according to input
        if (Spritz.GetKeyDown(KeyCode.UpArrow) || Spritz.GetKeyDown(KeyCode.W))
        {
            MoveSelectionUp();
        }
        else if (Spritz.GetKeyDown(KeyCode.DownArrow) || Spritz.GetKeyDown(KeyCode.S))
        {
            MoveSelectionDown();
        }
        else if (Spritz.GetKeyDown(KeyCode.LeftArrow) || Spritz.GetKeyDown(KeyCode.A))
        {
            MoveSelectionLeft();
        }
        else if (Spritz.GetKeyDown(KeyCode.RightArrow) || Spritz.GetKeyDown(KeyCode.D))
        {
            MoveSelectionRight();
        }
        else if (m_RevealedTimer > 0)
        {
            m_RevealedTimer--;
            OnRevealedTimerDone();
        }
        else if (Spritz.GetKeyDown(KeyCode.KeypadEnter) || Spritz.GetKeyDown(KeyCode.Return) || Spritz.GetKeyDown(KeyCode.Space))
        {
            TryActivateCurrentCard();
        }
        else if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            debugShowAllCards = !debugShowAllCards;
        }

    }

    private void InitPlayers()
    {
        baseLayerId = Spritz.CreateLayer("Spritesheets/Cards");
        m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        m_CurrentCardX = m_CurrentCardY = 0;

        var allMonsters = ExampleUtils.GetUfHeroes(Spritz.GetSprites());
        ExampleUtils.Shuffle(allMonsters);

        var playerCards = new MQ.CardModel[deckSize];
        for (var i = 0; i < deckSize; ++i)
            playerCards[i] = new MQ.CardModel() { sprite = allMonsters[i] };

        var overlordCards = new MQ.CardModel[deckSize];
        for (var i = 0; i < deckSize; ++i)
            overlordCards[i] = new MQ.CardModel() { sprite = allMonsters[deckSize + i] };

        m_Player = new MQ.Player()
        {
            name = "Seb",
            deck = new MQ.Deck()
            {
                cards = playerCards
            },
            hp = 10
        };

        m_Opponent = new MQ.Player()
        {
            name = "Opponent",
            deck = new MQ.Deck()
            {
                cards = overlordCards
            },
            hp = 12
        };

        m_CurrentPlayer = m_Player;
    }

    private void InitBoard()
    {
        m_Cards = new MQ.Card[nbCards];
        var cardIndex = 0;
        for (var i = 0; i < deckSize; i++, cardIndex += 4)
        {
            m_Cards[cardIndex] = new MQ.Card() { model = m_Opponent.deck.cards[i] };
            m_Cards[cardIndex + 1] = new MQ.Card() { model = m_Opponent.deck.cards[i] };
            m_Cards[cardIndex + 2] = new MQ.Card() { model = m_Player.deck.cards[i] };
            m_Cards[cardIndex + 3] = new MQ.Card() { model = m_Player.deck.cards[i] };
        }

        ExampleUtils.Shuffle(m_Cards);

        for (var cx = 0; cx < gridSize; ++cx)
        {
            for (var cy = 0; cy < gridSize; ++cy)
            {
                var x = cx * cardWidth + m_BoardRect.x;
                var y = cy * (cardHeight) + m_BoardRect.y;
                var i = cx + (cy * gridSize);
                m_Cards[i].x = x;
                m_Cards[i].y = y;
            }
        }
    }

    private void InitUI()
    {
        var playerZoneHeight = 50;
        var padding = 5;
        var totalRect = new RectInt(0, 0, resolution.x, resolution.y);
        var boardZone = totalRect.CutLeft(gridSize * (cardWidth + 5));
        var overlordZone = totalRect.CutTop(playerZoneHeight);
        var playerZone = totalRect.CutBottom(playerZoneHeight);
        var inspectorZone = totalRect;
        m_OpponentRect = overlordZone.AddPadding(padding);
        m_PlayerRect = playerZone.AddPadding(padding);
        m_BoardRect = boardZone.AddPadding(padding);
        m_InspectorRect = inspectorZone.AddPadding(5);
    }

    private void InitFont()
    {
        fontLayerId = Spritz.CreateLayer("Fonts/Weiholmir_GameMaker_sheet");
        var sprites = Spritz.GetSprites();
        m_Font = new SpriteFont(7, 7);
        m_Font.Add('!', (char)127, sprites, 1);
    }

    private void InitEffects()
    {
        effectLayerId = Spritz.CreateLayer();
        m_DissolveEffect = EffectsFactory.CreateDissolve(cardWidth, cardHeight, MQ.Theme.cardBackgroundColor, revealedDurationInSecond);
        m_DissolveEffect.playing = false;
    }

    private void MoveSelectionUp()
    {
        m_CurrentCardY -= 1;
        if (m_CurrentCardY == -1)
            m_CurrentCardY = gridSize - 1;
    }

    private void MoveSelectionDown()
    {
        m_CurrentCardY = (m_CurrentCardY + 1) % gridSize;
    }


    private void MoveSelectionLeft()
    {
        m_CurrentCardX -= 1;
        if (m_CurrentCardX == -1)
            m_CurrentCardX = gridSize - 1;
    }

    private void MoveSelectionRight()
    {
        m_CurrentCardX = (m_CurrentCardX + 1) % gridSize;
    }

    private void TryActivateCurrentCard()
    {
        if (m_RevealedCardIndex1 != currentCardIndex &&
            !m_Cards[currentCardIndex].isActivated &&
            ((m_CurrentPlayer == m_Player && m_Cards[currentCardIndex].state != MQ.CardState.ReadiedForOpponent) ||
             (m_CurrentPlayer == m_Opponent && m_Cards[currentCardIndex].state != MQ.CardState.ReadiedForPlayer))
            )
        {
            if (m_Cards[currentCardIndex].state < MQ.CardState.Revealed)
                m_Cards[currentCardIndex].state = MQ.CardState.Revealed;

            m_Cards[currentCardIndex].isActivated = true;
            if (m_RevealedCardIndex1 == -1)
            {
                m_RevealedCardIndex1 = currentCardIndex;
            }
            else
            {
                m_RevealedCardIndex2 = currentCardIndex;
                if (m_RevealedCardIndex1 != currentCardIndex && m_Cards[currentCardIndex].model == m_Cards[m_RevealedCardIndex1].model)
                {
                    // This is a match.
                    m_RevealedTimer = revealedDurationInFrames;
                    HandleMatch();
                }
                else
                {
                    m_RevealedTimer = revealedDurationInFrames;
                }
            }
        }
    }

    private void OnRevealedTimerDone()
    {
        if (m_RevealedTimer == 0)
        {
            m_DissolveEffect.playing = false;
            m_Cards[m_RevealedCardIndex1].isActivated = false;
            m_Cards[m_RevealedCardIndex2].isActivated = false;

            if (m_RevealedCardIndex1 != m_RevealedCardIndex2 && m_Cards[m_RevealedCardIndex1].model == m_Cards[m_RevealedCardIndex2].model)
            {
                // This is a match:
                m_Cards[m_RevealedCardIndex1].state = MQ.CardState.OutOfBoard;
                m_Cards[m_RevealedCardIndex2].state = MQ.CardState.OutOfBoard;
            }
            else
            {
                // No match
                if (m_Cards[m_RevealedCardIndex1].state < MQ.CardState.Revealed)
                    m_Cards[m_RevealedCardIndex1].state = MQ.CardState.Revealed;

                if (m_Cards[m_RevealedCardIndex1].state < MQ.CardState.Revealed)
                    m_Cards[m_RevealedCardIndex2].state = MQ.CardState.Revealed;

                // Check all Revealed cards if we have some that matches one of the 2 revealed cards. They become Ready:
                for (var i = 0; i < m_Cards.Length; ++i)
                {
                    if (m_Cards[i].state == MQ.CardState.Revealed)
                    {
                        var c = m_Cards[i];
                        var r1 = m_Cards[m_RevealedCardIndex1];
                        var r2 = m_Cards[m_RevealedCardIndex2];
                        var newState = m_CurrentPlayer == m_Player ? MQ.CardState.ReadiedForPlayer : MQ.CardState.ReadiedForOpponent;
                        if (i != m_RevealedCardIndex1 && r1 == c)
                        {
                            m_Cards[m_RevealedCardIndex1].state = newState;
                            m_Cards[i].state = newState;
                        }
                        else if (i != m_RevealedCardIndex2 && r2 == c)
                        {
                            m_Cards[m_RevealedCardIndex2].state = newState;
                            m_Cards[i].state = newState;
                        }
                    }
                }

                m_Cards[m_RevealedCardIndex1].model.sprite.Reset();
                m_Cards[m_RevealedCardIndex2].model.sprite.Reset();
            }

            SwitchPlayer();
            m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        }
    }

    private void HandleMatch()
    {
        if (m_CurrentPlayer == m_Opponent)
            m_Player.hp -= 2;
        else
            m_Opponent.hp -= 2;

        m_DissolveEffect.playing = true;
        m_DissolveEffect.Reset();
    }

    private void DrawDebugRect()
    {
        Spritz.DrawRectangle(m_OpponentRect.x, m_OpponentRect.y, m_OpponentRect.width, m_OpponentRect.height, Spritz.palette[5], false);
        Spritz.DrawRectangle(m_PlayerRect.x, m_PlayerRect.y, m_PlayerRect.width, m_PlayerRect.height, Spritz.palette[2], false);
        Spritz.DrawRectangle(m_BoardRect.x, m_BoardRect.y, m_BoardRect.width, m_BoardRect.height, Spritz.palette[3], false);
        Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
        
        // Spritz.DrawRectangle(0, 0, 600, 600, Spritz.palette[4], true);
    }

    private void DrawInspector()
    {
        if (m_CurrentCardY != -1 && m_CurrentCardX != -1)
        {
            var c = m_Cards[currentCardIndex];
            if (c.isVisible && c.isOnBoard)
            {
                c.model.sprite.Draw(m_InspectorRect.x, m_InspectorRect.y);

                if (debugMode)
                {
                    var activated = c.isActivated ? "- Activated" : "";
                    var status = $"{c.state} {activated}";
                    DrawText(status, m_InspectorRect.x, m_InspectorRect.y + 2 *spriteSize + 5);
                    Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
                }
            }
        }
    }

    private void SwitchPlayer()
    {
        // Switch player:
        if (m_CurrentPlayer == m_Player)
            m_CurrentPlayer = m_Opponent;
        else
            m_CurrentPlayer = m_Player;
    }
    
    private void DrawPlayerInfo(RectInt rect, MQ.Player p)
    {
        var currentPlayer = m_CurrentPlayer == p ? "*" : "";
        DrawText($"{p.name}  {p.hp}  {currentPlayer}", rect.x, rect.y);
    }

    private void DrawText(string text, int x, int y)
    {
        var currentLayer = Spritz.currentLayerId;
        Spritz.currentLayerId = 1;
        Spritz.Print(m_Font, text, x, y, Color.white);
        Spritz.currentLayerId = currentLayer;
    }

    private void DrawBoard()
    {
        for (var cx = 0; cx < gridSize; ++cx)
        {
            for (var cy = 0; cy < gridSize; ++cy)
            {
                var i = cx + (cy * gridSize);
                var card = m_Cards[i];
                if (card.isOnBoard)
                {
                    DrawCard(i, card.x, card.y);
                }
                if (m_CurrentCardX == cx && m_CurrentCardY == cy)
                {
                    Spritz.DrawRectangle(card.x, card.y, cardWidth, cardHeight, Color.yellow, false);
                }
                else if (m_Cards[i].isActivated)
                {
                    Spritz.DrawRectangle(card.x, card.y, cardWidth, cardHeight, Color.blue, false);
                }
            }
        }
    }

    private void DrawCard(int cardIndex, int x, int y)
    {
        Spritz.DrawRectangle(x + 1, y + 1, cardWidth - 2, cardHeight - 2, MQ.Theme.cardBackgroundColor, true);
        if (m_Cards[cardIndex].isVisible || debugShowAllCards)
        {
            var offsetX = (cardWidth - spriteSize) / 2;
            m_Cards[cardIndex].model.sprite.Draw(x + offsetX, y);
        }
    }

    private void DrawEffects()
    {
        if (m_DissolveEffect.playing && m_DissolveEffect.ticker.hasValue && m_RevealedCardIndex1 != -1 && m_RevealedCardIndex2 != -1)
        {
            Spritz.currentLayerId = effectLayerId;
            var c = m_Cards[m_RevealedCardIndex1];
            m_DissolveEffect.Draw(c.x, c.y);
            c = m_Cards[m_RevealedCardIndex2];
            m_DissolveEffect.Draw(c.x, c.y);
        }
    }
}
