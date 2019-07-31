
namespace AxelF {

[System.Serializable]
public struct TernaryBool {
    int value;

    public override bool Equals(object o) {
        return o is TernaryBool ? ((TernaryBool) o).value == value : false;
    }

    public override int GetHashCode() {
        return (value + 2) * 0x1357;
    }

    public bool ToBool(bool polarity) {
        return polarity ? value > 0 : value < 0;
    }

    public static implicit operator TernaryBool(bool b) {
        return new TernaryBool {value = (b ? 1 : -1)};
    }

    public static bool operator ==(TernaryBool a, bool b) { return a.ToBool(b); }
    public static bool operator !=(TernaryBool a, bool b) { return !a.ToBool(b); }
}

} // AxelF

