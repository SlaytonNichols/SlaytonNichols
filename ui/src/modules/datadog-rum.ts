import { datadogRum } from '@datadog/browser-rum'
import { UserModule } from '@/types'

export const install: UserModule = ({ isClient }) => {
  if (isClient) {
    datadogRum.init({
      applicationId: '6f4b27ea-61b6-4676-a1c0-96492230da6e',
      clientToken: 'pubd496da5f7fd8ca374c30db223be9a4a9',
      site: 'us3.datadoghq.com',
      service: 'slaytonnichols',
      sessionSampleRate: 100,
      sessionReplaySampleRate: 20,
      trackUserInteractions: true,
      trackResources: true,
      trackLongTasks: true,
      defaultPrivacyLevel: 'mask-user-input',
    })

    datadogRum.startSessionReplayRecording()
  }
}
