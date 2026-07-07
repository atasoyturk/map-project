import { useEffect, useRef } from "react";
import createGlobe from "cobe";

const TURKEY_MARKER = { location: [39.0, 35.0] as [number, number], size: 0.08 };

export function GlobeCanvas() {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    let phi = 2.2;
    let rafId: ReturnType<typeof requestAnimationFrame>;
    if (!canvasRef.current) return;

    const globe = createGlobe(canvasRef.current, {
      devicePixelRatio: 2,
      width: 400,
      height: 400,
      phi,
      theta: 0.2,
      dark: 1,
      diffuse: 1.8,
      mapSamples: 16000,
      mapBrightness: 7,
      baseColor: [0.1, 0.2, 0.5],
      markerColor: [0.98, 0.75, 0.14],
      glowColor: [0.2, 0.4, 1.0],
      markers: [TURKEY_MARKER],
    });

    function animate() {
      phi += 0.004;
      globe.update({ phi });
      rafId = requestAnimationFrame(animate);
    }

    rafId = requestAnimationFrame(animate);

    return () => {
      cancelAnimationFrame(rafId);
      globe.destroy();
    };
  }, []);

  return (
    <canvas
      ref={canvasRef}
      width={400}
      height={400}
      style={{ width: 200, height: 200, borderRadius: "50%" }}
    />
  );
}