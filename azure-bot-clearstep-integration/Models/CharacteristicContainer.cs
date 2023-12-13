using System.Collections.Generic;

namespace CustomDialogs.Models;

public class CharacteristicContainer
{
    public List<Characteristic> Primary { get; set; }
    public List<Characteristic> Secondary { get; set; }
    public List<Characteristic> Tertiary { get; set; }
}
