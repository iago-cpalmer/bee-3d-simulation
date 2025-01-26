
public class Globals
{
    // ///////////////////////////////////////
    // WORLD CONSTANTS
    // ///////////////////////////////////////
    public static readonly int WORLD_SIZE = 32;
    public static readonly int CHUNK_SIZE = 32;
    public static readonly int VIEW_CHUNK_RANGE = 30;
    public static readonly int BIT_CHUNK_COORD_OFFSET = 5;
    public static readonly int BIT_MASK_CHUNK_COORD = 0b11111;

    public static int SEED = 33;

    public static readonly float MAX_DISTANCE_FOR_SOUND = 100;

    public static readonly int N_LEVELS = 3;
    // ///////////////////////////////////////
    // WORLD GENERATION CONSTANTS
    // ///////////////////////////////////////
    public const int WATER_LEVEL = 20;

    public const int CONTINENTALNESS_OCTAVES = 6;
    public const float CONTINENTALNESS_PERSISTENCE = 0.5f;
    public const float CONTINENTALNESS_LACUNARITY = 2.0f;
    public const int CONTINENTALNESS_SCALE = 500;
    public const int CONTINENTALNESS_OFFSET = -10000;

    public const int EROSION_OCTAVES = 5;
    public const int EROSION_OFFSET = 10000;
    public const float EROSION_PERSISTENCE = 0.5f;
    public const float EROSION_LACUNARITY = 2f;
    public const int EROSION_SCALE = 100;

    public const int FLORA_OCTAVES = 4;
    public const int FLORA_OFFSET = 1000;
    public const float FLORA_PERSISTENCE = 0.5f;
    public const float FLORA_LACUNARITY = 2f;
    public const int FLORA_SCALE = 100;

    public static readonly int MAX_DECORATION_OBJECTS_PER_CHUNK = 1;

    // ///////////////////////////////////////
    // BEE BEHAVIOUR CONSTANTS
    // ///////////////////////////////////////
    public static float[] SIMULATION_SPEEDS = { 0.25f, 0.5f, 1.0f, 1.5f, 2.0f, 5.0f };
    public static int SIMULATION_SPEED_SELECTED = 2;
    public static bool SIMULATION_PAUSED;
    public static float SIMULATION_SPEED { get { return (SIMULATION_PAUSED?0:SIMULATION_SPEEDS[SIMULATION_SPEED_SELECTED]); } }
    public static readonly float TIME_FOR_HEIGHT_REQUEST = 5.0f;
    

    public static readonly float DAY_DURATION = 300; // 5 minutes each day
    public static readonly float TIME_TO_REGEN_POLEN = 5*DAY_DURATION;
    public static readonly float LIFE_DECREMENT_FACTOR_WHEN_TIRED = 3.0f;
    public static readonly float MAX_LIFE_EXPECTANCY = 25*DAY_DURATION;
    public static readonly float TIME_TO_GROW = 4*DAY_DURATION;
    public static readonly float MAX_TIME_LARVAE_CAN_SURVIVE_WITHOUT_FOOD = 1*DAY_DURATION;

    public static readonly float BEE_MAX_FORAGING_PITCH_ANGLE = 45;
    public static readonly float BEE_MAX_FORAGING_YAW_ANGLE_BIASED = 135;
    public static readonly float BEE_MAX_FORAGING_YAW_ANGLE = 180;
    public static readonly float[] BEE_FORAGING_STEPS = { 200, 50, 10};
    public static readonly float[] BEE_FORAGING_STEPS_PROBABILITIES = { 0.0f, 0.1f, 1.0f}; //10-0 = 10%, 100-10 = 90%

    public static readonly float BEE_COLLECTION_MIN_TIME = 5.0f;
    public static readonly float BEE_COLLECTION_MAX_TIME = 15.0f;
    public static readonly int MAX_BEE_POLLEN_TO_CARRY = 20;
    public static readonly int MAX_POLLEN_CONSUMPTION = 20;

    public static readonly float PROTEIN_CONSUMPTION_PER_SECOND = 0.01f;
    public static readonly float CARBS_CONSUMPTION_PER_SECOND = 0.03f;
    public static readonly float FATS_CONSUMPTION_PER_SECOND = 0.01f;
    public static readonly float LARVAE_PROTEIN_CONSUMPTION_PER_SECOND = 0.07f;
    public static readonly float LARVAE_CARBS_CONSUMPTION_PER_SECOND = 0.08f;
    public static readonly float LARVAE_FATS_CONSUMPTION_PER_SECOND = 0.05f;
    

    public static readonly float BEE_REST_MIN_TIME = 5.0f;
    public static readonly float BEE_REST_MAX_TIME = 15.0f;
    public static readonly float BEE_MAX_ENERGY = 300.0f; // 5 minutes

    public static readonly int DANCE_BUFFER_CAPACITY = 100;
    public static readonly float BEE_DANCE_MIN_TIME = 5.0f;
    public static readonly float BEE_DANCE_MAX_TIME = 15.0f;
    
    public static readonly float BEE_WATCH_MIN_TIME = 2.0f;
    public static readonly float BEE_WATCH_MAX_TIME = 5.0f;

    public static readonly int MIN_POLEN_AMOUNT = 30;
    public static readonly int MAX_POLEN_AMOUNT = 100;
    public static readonly int MIN_POLEN_PER_BEE = 25; // Minimun pollen stored per bee to be considered optimal to grow the colony

    public static readonly float STRESS_INCREMENT_MAX = 4.0f;
    public static readonly float STRESS_INCREMENT_MIN = 0.75f;
    public static readonly float MAX_STRESS_TO_DIE = 3000;

    // ///////////////////////////////////////
    // ANIMATION INDICES
    // ///////////////////////////////////////
    public static readonly int ANIMATION_BEE_FLY = 0;
    public static readonly int ANIMATION_BEE_DANCE = 1;

}
