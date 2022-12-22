using UniMini;
using UnityEngine;

public class UI : SpritzGame
{
    SpritzUI ui;
    string stateText;
    bool isToggle;
    SpritzCellLayout layout;
    SpriteId m_ImgButtonSprite;
    public override void InitializeSpritz()
    {
        // Create Layer and initialize various states
        Spritz.CreateLayer("Spritesheets/tiny_dungeon_monsters");
        m_ImgButtonSprite = Spritz.GetSprites()[0];

        ui = new SpritzUI();
        layout = new SpritzCellLayout();
    }

    public override void StartFrame()
    {
        ui.StartFrame();
    }

    public override void UpdateSpritz()
    {
        var l = layout;
        ui.Update();
        l.Reset(5, 5);

        // Update objects behavior according to input
        var res = ui.Button(l.Row(40, 15), "hello!");
        if (res.isHit)
        {
            isToggle = !isToggle;
        }
        stateText = $"H: {res.isHit} Hov: {res.isHovered} Act: {ui.active == res.id}";
        // Empty row
        l.Row();
        ui.Label(l.Row(), stateText);


        res = ui.Button(l.Row(40, 15), m_ImgButtonSprite);
        if (res.isHit)
        {
            isToggle = !isToggle;
        }
        stateText = $"H: {res.isHit} Hov: {res.isHovered} Act: {ui.active == res.id}";
        // Empty row
        l.Row();
        ui.Label(l.Row(), stateText);
    }

    public override void DrawSpritz()
    {
        Spritz.Clear(Color.black);
        ui.Draw();
    }
}
