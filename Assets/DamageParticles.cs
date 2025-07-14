using UnityEngine;

public class DamageParticles : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem bloodParticles;
    public ParticleSystem sparkParticles;

    [Header("Particle Settings")]
    public ParticleForce weakForce = new ParticleForce(5f, 15f, 0.5f);
    public ParticleForce mediumForce = new ParticleForce(10f, 25f, 1f);
    public ParticleForce strongForce = new ParticleForce(15f, 35f, 1.5f);

    [Header("Emission Settings")]
    public int weakEmissionCount = 10;
    public int mediumEmissionCount = 20;
    public int strongEmissionCount = 35;

    [System.Serializable]
    public class ParticleForce
    {
        public float minSpeed;
        public float maxSpeed;
        public float lifetime;

        public ParticleForce(float min, float max, float life)
        {
            minSpeed = min;
            maxSpeed = max;
            lifetime = life;
        }
    }

    public enum ParticleType
    {
        Blood,
        Sparks,
        Both
    }

    public enum ForceStrength
    {
        Weak,
        Medium,
        Strong
    }

    public void PlayDamageEffect(Vector3 position, Vector2 direction, ForceStrength strength, ParticleType type = ParticleType.Blood)
    {
        ParticleForce force = GetForceSettings(strength);
        int emissionCount = GetEmissionCount(strength);

        switch (type)
        {
            case ParticleType.Blood:
                PlayParticleSystem(bloodParticles, position, direction, force, emissionCount);
                break;
            case ParticleType.Sparks:
                PlayParticleSystem(sparkParticles, position, direction, force, emissionCount);
                break;
            case ParticleType.Both:
                PlayParticleSystem(bloodParticles, position, direction, force, emissionCount);
                PlayParticleSystem(sparkParticles, position, direction, force, emissionCount / 2);
                break;
        }
    }

    public void PlayDamageEffect(Vector3 position, float angle, ForceStrength strength, ParticleType type = ParticleType.Blood)
    {
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        PlayDamageEffect(position, direction, strength, type);
    }
    public void PlayDamageEffect(Vector3 position, Vector3 direction, ForceStrength strength, ParticleType type = ParticleType.Blood)
    {
        PlayDamageEffect(position, direction, strength, type);
    }

    private void PlayParticleSystem(ParticleSystem particles, Vector3 position, Vector2 direction, ParticleForce force, int count)
    {
        if (particles == null) return;
        var shape = particles.shape;
        var sabeShapeAngle = shape.angle;
        if (direction != Vector2.zero)
        {
            // cone if with direction
            shape.angle = 15f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            particles.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        }
        else
        {
            // circle if without direction
            shape.angle = 60f;
            particles.transform.rotation = Quaternion.Euler(0, -90f, -100f);
        }
        particles.Emit(count);
        shape.angle = sabeShapeAngle;
    }

    private ParticleForce GetForceSettings(ForceStrength strength)
    {
        switch (strength)
        {
            case ForceStrength.Weak: return weakForce;
            case ForceStrength.Medium: return mediumForce;
            case ForceStrength.Strong: return strongForce;
            default: return mediumForce;
        }
    }

    private int GetEmissionCount(ForceStrength strength)
    {
        switch (strength)
        {
            case ForceStrength.Weak: return weakEmissionCount;
            case ForceStrength.Medium: return mediumEmissionCount;
            case ForceStrength.Strong: return strongEmissionCount;
            default: return mediumEmissionCount;
        }
    }

    public void PlayWeakBloodEffect(Vector3 position, Vector2 direction = default)
    {
        PlayDamageEffect(position, direction, ForceStrength.Weak, ParticleType.Sparks);
    }

    public void PlayMediumSparkEffect(Vector3 position, Vector2 direction = default)
    {
        PlayDamageEffect(position, direction, ForceStrength.Medium, ParticleType.Both);
    }

    public void PlayStrongMixedEffect(Vector3 position, Vector2 direction = default)
    {
        PlayDamageEffect(position, direction, ForceStrength.Strong, ParticleType.Both);
    }
}