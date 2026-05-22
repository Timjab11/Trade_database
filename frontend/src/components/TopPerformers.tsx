import React from 'react';
import { Box, Stack, Typography, Grid } from '@mui/material';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import LeaderboardCard from './LeaderboardCard';
import { Performer } from './types';

interface TopPerformersProps {
  performers: Performer[];
}

export default function TopPerformers({ performers }: TopPerformersProps) {
  if (performers.length === 0) return null;

  return (
    <Box sx={{ mb: 4 }}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', mb: 2 }}>
        <EmojiEventsIcon color="action" />
        <Typography variant="subtitle1" color="textSecondary" sx={{ fontWeight: 'bold' }}>
          Pořadí obchodníků (TOP 6)
        </Typography>
      </Stack>

      <Grid container spacing={2}>
        {performers.map((person) => (
          <Grid size={{ xs: 12, sm: 6, md: 4, xl: 2 }} key={person.rank}>
            <LeaderboardCard person={person} />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}