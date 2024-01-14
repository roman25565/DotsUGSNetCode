using System;
public class PlayerDataDynamic
{
    public PlayerDataDynamic(float gold, float globalSkills)
    {
        _gold = gold;
        _globalSkills = globalSkills;
    }
    
    
    private float _gold;  // Underscore for private fields
    private float _globalSkills;
    public float Gold
    {
        get => _gold;
        private set  // Enforce encapsulation
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Gold cannot be negative.");
            }
            _gold = value;
        }
    }

    public void AddGold(float value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Amount to add cannot be negative.");
        }
        Gold += value;  // Leverage property for validation and logic
    }

    public void WithdrawGold(float value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Amount to withdraw cannot be negative.");
        }
        if (value > Gold)
        {
            throw new InvalidOperationException("Insufficient gold to withdraw.");
        }
        Gold -= value;
    }
    
    
}