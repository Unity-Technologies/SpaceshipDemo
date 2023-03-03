using System.Collections.Generic;

namespace GameplayIngredients.Interactions
{
    public static class InteractionManager
    {
        public static IEnumerable<Interactive> interactives { get { return s_Interactives; } }

        static List<Interactive> s_Interactives = new List<Interactive>();
        
        public static void RegisterInteractive(Interactive interactive)
        {
            s_Interactives.Add(interactive);
        }

        public static void RemoveInteractive(Interactive interactive)
        {
            s_Interactives.Remove(interactive);
        }

        public static void Interact(InteractiveUser user)
        {
            foreach(var interactive in GetCandidates(user))
            {
                if (interactive.Interact(user))
                    break;
            }
        }

        public static Interactive[] GetCandidates(InteractiveUser user)
        {
            List<Interactive> candidates = new List<Interactive>();

            foreach(var interactive in s_Interactives) 
            {
                // Filter interactives at user's reach

                if (interactive.CanInteract(user))
                    candidates.Add(interactive);
            }

            return user.SortCandidates(candidates.ToArray());
        }

    }
}
