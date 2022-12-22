using UniMini;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mem
{
    struct Card
    {
        public AnimSprite sprite;
        public bool isVisible;
        public bool valid;
    }
}

public class Memory : SpritzGame
{
    Mem.Card[] m_Cards;
    int m_CurrentCardX;
    int m_CurrentCardY;
    int m_RevealedCardIndex1;
    int m_RevealedCardIndex2;
    int m_RevealedTimer;
    const int gridSize = 8;
    const int nbSprites = 8 * 8;
    const int spriteSize = 16;
    bool m_DebugMode;

    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        m_CurrentCardX = m_CurrentCardY = 0;
        var allSprites = Spritz.GetSprites();
        m_Cards = new Mem.Card[nbSprites];

        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;
        var allMonsters = ExampleUtils.GetTinyMonsters(Spritz.GetSprites());
        var monsters = new AnimSprite[32];
        var monsterIndex = 0;
        for (var i = 0; i < monsters.Length; ++i, monsterIndex += 4)
        {
            monsters[i] = allMonsters[monsterIndex];
        }

        for(var i = 0; i < nbSprites; i += 2)
        {
            var card = new Mem.Card()
            {
                sprite = monsters[i / 2],
                isVisible = false,
                valid = true
            };

            m_Cards[i] = card;
            m_Cards[i + 1] = card;
        }

        ExampleUtils.Shuffle(m_Cards);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        if (Spritz.GetKeyDown(KeyCode.UpArrow) || Spritz.GetKeyDown(KeyCode.W))
        {
            m_CurrentCardY -= 1;
            if (m_CurrentCardY == -1)
                m_CurrentCardY = gridSize - 1;
        }
        else if (Spritz.GetKeyDown(KeyCode.DownArrow) || Spritz.GetKeyDown(KeyCode.S))
        {
            m_CurrentCardY = (m_CurrentCardY + 1) % gridSize;

        }
        else if (Spritz.GetKeyDown(KeyCode.LeftArrow) || Spritz.GetKeyDown(KeyCode.A))
        {
            m_CurrentCardX -= 1;
            if (m_CurrentCardX == -1)
                m_CurrentCardX = gridSize - 1;
        }
        else if (Spritz.GetKeyDown(KeyCode.RightArrow) || Spritz.GetKeyDown(KeyCode.D))
        {
            m_CurrentCardX = (m_CurrentCardX + 1) % gridSize;
        }
        else if (m_RevealedTimer > 0)
        {
            m_RevealedTimer--;
            if (m_RevealedTimer == 0)
            {
                if (m_Cards[m_RevealedCardIndex2].sprite.frames[0] == m_Cards[m_RevealedCardIndex1].sprite.frames[0])
                {
                    // it is a match
                    m_Cards[m_RevealedCardIndex2].valid = false;
                    m_Cards[m_RevealedCardIndex1].valid = false;
                }
                else
                {
                    m_Cards[m_RevealedCardIndex1].isVisible = false;
                    m_Cards[m_RevealedCardIndex2].isVisible = false;

                    m_Cards[m_RevealedCardIndex1].sprite.Reset();
                    m_Cards[m_RevealedCardIndex2].sprite.Reset();
                }
                m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
            }
        }
        else if (Spritz.GetKeyDown(KeyCode.KeypadEnter) || Spritz.GetKeyDown(KeyCode.Return) || Spritz.GetKeyDown(KeyCode.Space))
        {
            var currentCardIndex = m_CurrentCardX + m_CurrentCardY * gridSize;
            if (m_RevealedCardIndex1 != currentCardIndex && !m_Cards[currentCardIndex].isVisible)
            {
                m_Cards[currentCardIndex].isVisible = true;
                if (m_RevealedCardIndex1 == -1) 
                {
                    m_RevealedCardIndex1 = currentCardIndex;
                }
                else
                {
                    m_RevealedTimer = 45;
                    m_RevealedCardIndex2 = currentCardIndex;
                }
            }
        }
        else if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            m_DebugMode = !m_DebugMode;
            for (var c = 0; c < m_Cards.Length; ++c)
            {
                if (m_Cards[c].valid)
                    m_Cards[c].isVisible = m_DebugMode;
            }
            m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        }

        for (var c = 0; c < m_Cards.Length; ++c)
        {
            if (m_Cards[c].isVisible)
                m_Cards[c].sprite.Update();
        }
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.grey);
        for (var cx = 0; cx < gridSize; ++cx)
        {
            for (var cy = 0; cy < gridSize; ++cy)
            {
                var x = cx * spriteSize;
                var y = cy * spriteSize;
                var i = cx + (cy * gridSize);
                if (m_Cards[i].valid)
                {
                    if (m_Cards[i].isVisible)
                        m_Cards[i].sprite.Draw(x, y);
                    else
                    {
                        Spritz.DrawRectangle(x + 1, y + 1, spriteSize - 2, spriteSize - 2, Color.blue, false);
                    }
                }
                if (m_CurrentCardX == cx && m_CurrentCardY == cy)
                    Spritz.DrawRectangle(x, y, spriteSize, spriteSize, Color.yellow, false);
            }
        }
    }
}
