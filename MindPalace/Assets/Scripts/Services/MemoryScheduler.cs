using System;

/// <summary>
/// SM-2 lite spaced repetition scheduler.
/// Grades: Again (fail) / Hard / Good / Easy
/// Failed cards reset to 1 day; successful cards follow 1d → 6d → interval*ease progression.
/// </summary>
public static class MemoryScheduler {
	public enum Grade { Again = 0, Hard = 2, Good = 3, Easy = 4 }

	public static void InitializeIfNew(MemoryCard card) {
		if (card.ease <= 0) card.ease = 2.5;
		if (string.IsNullOrEmpty(card.dueIsoUtc)) card.DueUtc = DateTime.UtcNow;
	}

	public static void ApplyGrade(MemoryCard card, Grade grade, DateTime nowUtc) {
		InitializeIfNew(card);
		int q = (int)grade;

		if (q < 3) {
			// FAIL: Reset repetitions and schedule for tomorrow
			card.reps = 0;
			card.lapses += 1;
			card.intervalDays = 1;
			// Reduce ease factor (minimum 1.3)
			card.ease = Math.Max(1.3, card.ease + (0.1 - (3 - q) * (0.08 + (3 - q) * 0.02)));
		} else {
			// SUCCESS: Progress through intervals
			card.reps += 1;
			if (card.reps == 1) {
				card.intervalDays = 1;  // First success: 1 day
			} else if (card.reps == 2) {
				card.intervalDays = 6;  // Second success: 6 days
			} else {
				// Subsequent: multiply by ease factor
				card.intervalDays = (int)Math.Round(card.intervalDays * card.ease);
			}
			// Adjust ease based on grade difficulty
			double deltaEase = 0.1 - (3 - q) * (0.08 + (3 - q) * 0.02);
			card.ease = Math.Max(1.3, card.ease + deltaEase);
		}

		// Set next review date
		card.DueUtc = nowUtc.AddDays(card.intervalDays);
	}
}
